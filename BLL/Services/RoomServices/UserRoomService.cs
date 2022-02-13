﻿using BLL.Abstractions.Interfaces.RoleInterfaces;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Enums;
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

        public UserRoomService(
            IRoomService roomService,
            IUserService userService,
            IUserRoomRoleService roleService,
            IRoomValidationService validationService,
            ITransactionsWorker transactionsWorker)
        {
            this.transactionsWorker = transactionsWorker;
            this.roomService = roomService;
            this.userService = userService;
            this.roleService = roleService;
            this.validationService = validationService;
        }

        public async Task<ExceptionalResult> CreateRoomForUser(UserModel user, RoomCreateModel createModel, bool asTransaction = true)
        {
            return asTransaction
                ? await this.transactionsWorker.RunAsTransaction(() => this.InnerCreateRoomForUser(user, createModel))
                : await this.CreateRoomForUser(user, createModel);
        }

        public async Task<ExceptionalResult> DeleteRoomByUser(UserModel user, int roomId, bool asTransaction = true)
        {
            return asTransaction
                ? await this.transactionsWorker.RunAsTransaction(() => this.InnerDeleteRoomByUser(user, roomId))
                : await this.InnerDeleteRoomByUser(user, roomId);
        }

        public async Task<ExceptionalResult> DeleteUserAndRooms(UserModel user, bool asTransaction = true)
        {
            return asTransaction
                ? await this.transactionsWorker.RunAsTransaction(() => this.InnerDeleteUserAndRooms(user))
                : await this.InnerDeleteUserAndRooms(user);
        }

        public async Task<ExceptionalResult> DeleteUserFromRoom(UserModel user, RoomModel room, bool asTransaction = true)
        {
            return asTransaction
                ? await this.transactionsWorker.RunAsTransaction(() => this.InnerDeleteUserFromRoom(user, room))
                : await this.InnerDeleteUserFromRoom(user, room);
        }

        public async Task<ExceptionalResult> AddUserToRoom(string email, RoomModel room, bool asTransaction = true)
        {
            return asTransaction
                ? await this.transactionsWorker.RunAsTransaction(() => this.InnerAddUserToRoom(email, room))
                : await this.InnerAddUserToRoom(email, room);
        }

        public async Task<ExceptionalResult> UpdateRoomForUser(UserModel user, RoomEditModel editModel, bool asTransaction = true)
        {
            return asTransaction
                ? await this.transactionsWorker.RunAsTransaction(() => this.InnerUpdateRoomForUser(user, editModel))
                : await this.InnerUpdateRoomForUser(user, editModel);
        }

        private async Task<ExceptionalResult> InnerUpdateRoomForUser(UserModel user, RoomEditModel editModel)
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

            return !updateResult.IsSuccess ? updateResult : new ExceptionalResult();
        }

        private async Task<ExceptionalResult> InnerCreateRoomForUser(UserModel user, RoomCreateModel createModel)
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
            user.Rooms.Add(room);
            var updateResult = await this.UpdateUserRooms(user);
            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            return await this.roleService.AddRoleForUserAndRoom(user, room, Role.ADMIN.ToString(), false);
        }

        private async Task<ExceptionalResult> InnerDeleteRoomByUser(UserModel user, int roomId)
        {
            var roomResult = await this.ValidateUserAndRoomId(user, roomId);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var room = roomResult.Value;
            foreach (var roomUser in room.Users.ToHashSet())
            {
                var roleResult = await this.roleService.DeleteRoleForUserAndRoom(roomUser, room, false);
                if (!roleResult.IsSuccess)
                {
                    return roleResult;
                }

                roomUser.Rooms.Remove(room);
                var updateResult = await this.UpdateUserRooms(roomUser);
                if (!updateResult.IsSuccess)
                {
                    return updateResult;
                }
            }

            var deleteResult = await this.roomService.Delete(room.Id);

            return !deleteResult.IsSuccess ? deleteResult : new ExceptionalResult();
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
                    room.Users.Remove(user);
                    var updateResult = await this.UpdateRoomUsers(room);
                    if (!updateResult.IsSuccess)
                    {
                        return updateResult;
                    }

                    var roleResult = await this.roleService.DeleteRoleForUserAndRoom(user, room, false);
                    if (!roleResult.IsSuccess)
                    {
                        return roleResult;
                    }
                }
            }

            var deleteResult = await this.userService.Delete(user.Id);

            return !deleteResult.IsSuccess ? deleteResult : new ExceptionalResult();
        }

        private async Task<ExceptionalResult> InnerDeleteUserFromRoom(UserModel user, RoomModel room)
        {
            var userRole = await this.roleService.GetRoleForUserAndRoom(user, room);
            if (userRole is null)
            {
                return new ExceptionalResult(false, $"User with id {user.Id} does not belong to room with id {room.Id}");
            }

            if (userRole.RoleType.Name == Role.ADMIN.ToString())
            {
                if (await this.roleService.IsUserLastAdminInRoom(user, room))
                {
                    return new ExceptionalResult(false, "Room must have at least one admin. Make someone else admin first");
                }
            }

            var deleteResult = await this.roleService.DeleteRoleForUserAndRoom(user, room, false);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            user.Rooms.Remove(room);
            var userResult = await this.UpdateUserRooms(user);
            if (!userResult.IsSuccess)
            {
                return userResult;
            }

            room.Users.Remove(user);
            var roomResult = await this.UpdateRoomUsers(room);

            return !roomResult.IsSuccess ? roomResult : new ExceptionalResult();
        }

        private async Task<ExceptionalResult> InnerAddUserToRoom(string email, RoomModel room)
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

            user.Rooms.Add(room);
            var userResult = await this.UpdateUserRooms(user);
            if (!userResult.IsSuccess)
            {
                return userResult;
            }

            room.Users.Add(user);
            var roomResult = await this.UpdateRoomUsers(room);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            return await this.roleService.AddRoleForUserAndRoom(user, room, Role.MEMBER.ToString(), false);
        }

        private async Task<OptionalResult<RoomModel>> CreateRoom(RoomCreateModel createModel, UserModel user)
        {
            var creationResult = await this.roomService.Create(createModel);
            if (!creationResult.IsSuccess)
            {
                return creationResult;
            }

            var room = creationResult.Value;
            room.Users.Add(user);

            return await this.UpdateRoomUsers(room);
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
            if (!room.Users.Contains(user) || userRole.RoleType.Name != Role.ADMIN.ToString())
            {
                return new OptionalResult<RoomModel>(false, $"User with id {user.Id} does not have rights for this operation or does not belong to room with id {room.Id}");
            }

            return new OptionalResult<RoomModel>(room);
        }
    }
}
