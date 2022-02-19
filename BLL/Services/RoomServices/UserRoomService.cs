using BLL.Abstractions.Interfaces.AuditInterfaces;
using BLL.Abstractions.Interfaces.RoleInterfaces;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.AuditModels;
using Core.Models.RoomModels;
using Core.Models.UserModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.RoomServices
{
    internal class UserRoomService : IUserRoomService
    {
        private readonly IRoomService roomService;

        private readonly IUserService userService;

        private readonly IUserRoomRoleService roleService;

        private readonly IRoomValidationService validationService;

        private readonly ITransactionsWorker transactionsWorker;

        private readonly IAuditService auditService;

        public UserRoomService(
            IAuditService auditService,
            IRoomService roomService,
            IUserService userService,
            IUserRoomRoleService roleService,
            IRoomValidationService validationService,
            ITransactionsWorker transactionsWorker)
        {
            this.auditService = auditService;
            this.transactionsWorker = transactionsWorker;
            this.roomService = roomService;
            this.userService = userService;
            this.roleService = roleService;
            this.validationService = validationService;
        }

        public async Task<ExceptionalResult> CreateRoomForUser(UserModel user, RoomCreateModel createModel)
        {
            return await this.transactionsWorker.RunAsTransaction(() => this.InnerCreateRoomForUser(user, createModel));
        }

        public async Task<ExceptionalResult> DeleteRoomByUser(UserModel user, int roomId)
        {
            return await this.transactionsWorker.RunAsTransaction(() => this.InnerDeleteRoomByUser(user, roomId));
        }

        public async Task<ExceptionalResult> DeleteUserAndRooms(UserModel user)
        {
            return await this.transactionsWorker.RunAsTransaction(() => this.InnerDeleteUserAndRooms(user));
        }

        public async Task<ExceptionalResult> DeleteUserFromRoom(UserModel user, RoomModel room, UserModel userToDelete)
        {
            return await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerDeleteUserFromRoom(user, room, userToDelete));
        }

        public async Task<ExceptionalResult> AddUserToRoom(string email, RoomModel room,  UserModel actor)
        {
            return await this.transactionsWorker.RunAsTransaction(() => this.InnerAddUserToRoom(email, room, actor));
        }

        public async Task<ExceptionalResult> UpdateRoomForUser(UserModel user, RoomEditModel editModel)
        {
            return await this.transactionsWorker.RunAsTransaction(() => this.InnerUpdateRoomForUser(user, editModel));
        }

        private async Task<ExceptionalResult> InnerUpdateRoomForUser(UserModel user, RoomEditModel editModel)
        {
            var roomResult = await this.ValidateUserAndRoomId(user, editModel.Id);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var room = roomResult.Value;
            var updateModel = new RoomUpdateModel()
            {
                Id = editModel.Id,
                Name = editModel.Name,
            };
            var validationResult = this.validationService.ValidateUpdateModel(updateModel);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var updateResult = await this.roomService.Update(updateModel);

            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            var record = new CreateAuditRecordModel()
            {
                ActionType = ActionType.EditRoomInfo,
                Actor = user,
                Room = room,
            };

            return await this.auditService.CreateAuditRecord(record);
        }

        private async Task<ExceptionalResult> InnerCreateRoomForUser(UserModel user, RoomCreateModel createModel)
        {
            var validationResult = this.validationService.ValidateCreateModel(createModel);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            createModel.Users = new List<UserModel>() { user };
            var roomResult = await this.roomService.Create(createModel);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var room = roomResult.Value;

            return await this.roleService.AddRoleForUserAndRoom(user, room, RoleType.Admin.ToString(), false);
        }

        private async Task<ExceptionalResult> InnerDeleteRoomByUser(UserModel user, int roomId)
        {
            var roomResult = await this.ValidateUserAndRoomId(user, roomId);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var room = roomResult.Value;
            return await this.roomService.Delete(room.Id);
        }

        private async Task<ExceptionalResult> InnerDeleteUserAndRooms(UserModel user)
        {
            var rooms = user.Rooms.ToHashSet();
            foreach (var room in rooms)
            {
                if (room.Users.ToHashSet().Count == 1 || await this.roleService.IsUserLastAdminInRoom(user, room))
                {
                    var deletionResult = await this.InnerDeleteRoomByUser(user, room.Id);
                    if (!deletionResult.IsSuccess)
                    {
                        return deletionResult;
                    }
                }
                else
                {
                    var record = new CreateAuditRecordModel()
                    {
                        ActionType = ActionType.UserLeftFromRoom,
                        Actor = user,
                        Room = room,
                    };

                    var recordResult = await this.auditService.CreateAuditRecord(record);
                    if (!recordResult.IsSuccess)
                    {
                        return recordResult;
                    }
                }
            }

            return await this.userService.Delete(user.Id);
        }

        private async Task<ExceptionalResult> InnerDeleteUserFromRoom(UserModel user, RoomModel room, UserModel userToDelete)
        {
            var userRole = await this.roleService.GetRoleForUserAndRoom(user, room);
            if (userRole is null || userRole.RoleType.Name != RoleType.Admin.ToString())
            {
                return new ExceptionalResult(false, $"User with id {user.Id} does not belong to room with id {room.Id} or does not have rights for such action.");
            }

            userRole = await this.roleService.GetRoleForUserAndRoom(userToDelete, room);
            if (userRole is null)
            {
                return new ExceptionalResult(false, $"User with id {user.Id} does not belong to room with id {room.Id}");
            }

            if (userRole.RoleType.Name == RoleType.Admin.ToString())
            {
                if (await this.roleService.IsUserLastAdminInRoom(user, room))
                {
                    return new ExceptionalResult(false, "Room must have at least one admin. Make someone else admin first.");
                }
            }

            room.Users.Remove(userToDelete);
            var roomResult = await this.UpdateRoomUsers(room);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var record = new CreateAuditRecordModel()
            {
                ActionType = ActionType.DeleteUserFromRoom,
                Actor = user,
                Room = room,
                UserUnderAction = userToDelete,
            };
            return await this.auditService.CreateAuditRecord(record);
        }

        private async Task<ExceptionalResult> InnerAddUserToRoom(string email, RoomModel room, UserModel actor)
        {
            var actorRole = await this.roleService.GetRoleForUserAndRoom(actor, room);
            if (actorRole is null || actorRole.RoleType.Name != RoleType.Admin.ToString())
            {
                return new ExceptionalResult(false, $"User {actor.Id} does not have rights for this action");
            }

            var user = (await this.userService.GetActiveUsers(x => x.Email == email)).FirstOrDefault();
            if (user is null)
            {
                return new ExceptionalResult(false, $"User with email {email} does not exist");
            }

            var userRole = await this.roleService.GetRoleForUserAndRoom(user, room);
            if (userRole is not null)
            {
                return new ExceptionalResult(false, $"User with email {email} already in room {room.Id}");
            }

            room.Users.Add(user);
            var roomResult = await this.UpdateRoomUsers(room);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var roleResult = await this.roleService.AddRoleForUserAndRoom(user, room, RoleType.Member.ToString(), false);
            if (!roleResult.IsSuccess)
            {
                return roleResult;
            }

            var record = new CreateAuditRecordModel()
            {
                ActionType = ActionType.AddUserToRoom,
                Actor = actor,
                Room = room,
                UserUnderAction = user,
            };

            return await this.auditService.CreateAuditRecord(record);
        }

        private async Task<OptionalResult<RoomModel>> UpdateRoomUsers(RoomModel room)
        {
            var updateModel = new RoomUpdateModel()
            {
                Id = room.Id,
                Users = room.Users,
            };

            return await this.roomService.Update(updateModel);
        }

        private async Task<OptionalResult<RoomModel>> ValidateUserAndRoomId(UserModel user, int roomId)
        {
            var room = await this.roomService.GetRoomById(roomId);
            if (room is null)
            {
                return new OptionalResult<RoomModel>(false, $"Room with id {roomId} does not exist");
            }

            var userRole = await this.roleService.GetRoleForUserAndRoom(user, room);
            if (!room.Users.Contains(user) || userRole.RoleType.Name != RoleType.Admin.ToString())
            {
                return new OptionalResult<RoomModel>(false, $"User with id {user.Id} does not have rights for this operation or does not belong to room with id {room.Id}");
            }

            return new OptionalResult<RoomModel>(room);
        }
    }
}
