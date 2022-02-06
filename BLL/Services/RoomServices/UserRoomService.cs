using System.Linq;
using BLL.Abstractions.Interfaces.RoleInterfaces;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Services.RoomServices
{
    internal class UserRoomService : IUserRoomService
    {
        private readonly IRoomService roomService;

        private readonly IUserService userService;

        private readonly IUserRoomRoleService roleService;

        private readonly IRoomValidationService validationService;

        public UserRoomService(IRoomService roomService, IUserService userService, IUserRoomRoleService roleService, IRoomValidationService validationService)
        {
            this.roomService = roomService;
            this.userService = userService;
            this.roleService = roleService;
            this.validationService = validationService;
        }

        public async Task<IEnumerable<RoomModel>> GetRoomsForUser(UserModel user)
        {
            var tasks = await Task.WhenAll(user.Rooms.Select(id => this.roomService.GetRoomById(id)));
            return tasks.Where(result => result is not null);
        }

        public async Task<IEnumerable<UserModel>> GetUsersInRoom(RoomModel room)
        {
            var tasks = await Task.WhenAll(room.Users.Select(id => this.userService.GetUserById(id)));
            return tasks.Where(result => result is not null);
        }

        public async Task<ExceptionalResult> CreateRoomForUser(UserModel user, RoomCreateModel createModel)
        {
            var validationResult = this.validationService.ValidateCreateModel(createModel);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var roomResult = await this.CreateRoom(createModel, user);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var room = roomResult.Value;
            user.Rooms.Add(room.Id);
            var updateResult = await this.UpdateUserRooms(user);
            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            return await this.roleService.AddRoleForUserAndRoom(user, room, Role.ADMIN);
        }

        public async Task<ExceptionalResult> DeleteRoomByUser(UserModel user, int roomId)
        {
            var roomResult = await this.ValidateUserAndRoomId(user, roomId);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var room = roomResult.Value;
            foreach (var roomUser in await this.GetUsersInRoom(room))
            {
                var roleResult = await this.roleService.DeleteRoleForUserAndRoom(roomUser, room);
                if (!roleResult.IsSuccess)
                {
                    return roleResult;
                }

                roomUser.Rooms.Remove(room.Id);
                var updateResult = await this.UpdateUserRooms(roomUser);
                if (!updateResult.IsSuccess)
                {
                    return updateResult;
                }
            }

            var deleteResult = await this.roomService.Delete(room.Id);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            return new ExceptionalResult();
        }

        public async Task<ExceptionalResult> UpdateRoomForUser(UserModel user, RoomEditModel editModel)
        {
            var roomResult = await this.ValidateUserAndRoomId(user, editModel.Id);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

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

            return new ExceptionalResult();
        }

        public async Task<ExceptionalResult> DeleteUserFromRoom(UserModel user, RoomModel room)
        {
            var userRole = await this.roleService.GetRoleForUserAndRoom(user, room);
            if (userRole is null)
            {
                return new ExceptionalResult(false, $"User with id {user.Id} does not belong to room with id {room.Id}");
            }

            if (userRole.Role == Role.ADMIN)
            {
                var checkResult = await this.roleService.CheckOneAdminInRoom(user, room);
                if (!checkResult.IsSuccess)
                {
                    return checkResult;
                }
            }

            var deleteResult = await this.roleService.DeleteRoleForUserAndRoom(user, room);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            user.Rooms.Remove(room.Id);
            var userResult = await this.UpdateUserRooms(user);
            if (!userResult.IsSuccess)
            {
                return userResult;
            }

            room.Users.Remove(user.Id);
            var roomResult = await this.UpdateRoomUsers(room);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            return new ExceptionalResult();
        }

        public async Task<ExceptionalResult> AddUserToRoom(string email, RoomModel room)
        {
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

            user.Rooms.Add(room.Id);
            var userResult = await this.UpdateUserRooms(user);
            if (!userResult.IsSuccess)
            {
                return userResult;
            }

            room.Users.Add(user.Id);
            var roomResult = await this.UpdateRoomUsers(room);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            return await this.roleService.AddRoleForUserAndRoom(user, room, Role.MEMBER);
        }

        private async Task<OptionalResult<RoomModel>> CreateRoom(RoomCreateModel createModel, UserModel user)
        {
            createModel.Users = new List<int> { user.Id };

            return await this.roomService.Create(createModel);
        }

        private async Task<OptionalResult<UserModel>> UpdateUserRooms(UserModel user)
        {
            var updateModel = new UserUpdateModel()
            {
                Id = user.Id,
                Rooms = user.Rooms,
                IsActive = user.IsActive,
            };

            return await this.userService.Update(updateModel);
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

            if (!room.Users.Contains(user.Id) || userRole.Role != Role.ADMIN)
            {
                return new OptionalResult<RoomModel>(false, $"User with id {user.Id} does not have rights for this operation or does not belong to room with id {room.Id}");
            }

            return new OptionalResult<RoomModel>(room);
        }
    }
}
