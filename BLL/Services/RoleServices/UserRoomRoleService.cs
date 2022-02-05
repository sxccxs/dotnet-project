using BLL.Abstractions.Interfaces.RoleInterfaces;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Services.RoleServices
{
    internal class UserRoomRoleService : IUserRoomRoleService
    {
        private readonly IRoomService roomService;

        private readonly IUserService userService;

        private readonly IRoleService roleService;

        private readonly IRoleTypeService roleTypeService;

        public UserRoomRoleService(IRoomService roomService, IUserService userService, IRoleService roleService, IRoleTypeService roleTypeService)
        {
            this.roomService = roomService;
            this.userService = userService;
            this.roleService = roleService;
            this.roleTypeService = roleTypeService;
        }

        public async Task<RoleModel> GetRoleForUserAndRoom(UserModel user, RoomModel room)
        {
            return (await this.roleService.GetByConditions(r => r.User == user, r => r.Room == room)).FirstOrDefault();
        }

        public async Task<ExceptionalResult> AddRoleForUserAndRoom(UserModel user, RoomModel room, string roleName)
        {
            var roleTypeResult = await this.GetRoleTypeByName(roleName);
            if (!roleTypeResult.IsSuccess)
            {
                return roleTypeResult;
            }

            var roleType = roleTypeResult.Value;
            var role = new RoleModel()
            {
                RoleType = roleType,
                User = user,
                Room = room,
            };

            var roleResult = await this.roleService.CreateRole(role);
            if (!roleResult.IsSuccess)
            {
                return roleResult;
            }

            role = roleResult.Value;
            var addResult = await this.AddRoleModelToUserAndRoom(user, room, role);
            if (!addResult.IsSuccess)
            {
                return addResult;
            }

            return new ExceptionalResult();
        }

        public async Task<OptionalResult<RoleModel>> DeleteRoleForUserAndRoom(UserModel user, RoomModel room)
        {
            var role = await this.GetRoleForUserAndRoom(user, room);

            user.Roles.Remove(role);
            var userResult = await this.UpdateUserRoles(user);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<RoleModel>(userResult);
            }

            room.Roles.Remove(role);
            var roomResult = await this.UpdateRoomRoles(room);
            if (!roomResult.IsSuccess)
            {
                return new OptionalResult<RoleModel>(roomResult);
            }

            var roleResult = await this.roleService.Delete(role.Id);
            if (!roleResult.IsSuccess)
            {
                return new OptionalResult<RoleModel>(roleResult);
            }

            return new OptionalResult<RoleModel>(role);
        }

        public async Task<ExceptionalResult> UpdateRoleForUser(UserModel user, RoomModel room, string roleName)
        {
            var roleTypeResult = await this.GetRoleTypeByName(roleName);
            if (!roleTypeResult.IsSuccess)
            {
                return roleTypeResult;
            }

            var roleType = roleTypeResult.Value;

            var userRole = await this.GetRoleForUserAndRoom(user, room);
            if (userRole is null)
            {
                return new ExceptionalResult(false, $"User or room does not exist or user {user.Id} does not belong to room {room.Id}");
            }

            if (roleType.Name != Role.ADMIN.ToString())
            {
                if (await this.IsUserLastAdminInRoom(user, room))
                {
                    return new ExceptionalResult(false, "Room must have at least one admin. Make someone else admin first");
                }
            }

            userRole.RoleType = roleType;
            await this.roleService.Update(userRole);

            return new ExceptionalResult();
        }

        public async Task<bool> IsUserLastAdminInRoom(UserModel user, RoomModel room)
        {
            return !(await this.roleService.GetByConditions(r => r.Room == room)).Any(r =>
                r.RoleType.Name == Role.ADMIN.ToString() && r.User != user);
        }

        private async Task<ExceptionalResult> AddRoleModelToUserAndRoom(UserModel user, RoomModel room, RoleModel role)
        {
            user.Roles.Add(role);
            var userUpdateResult = await this.UpdateUserRoles(user);
            if (!userUpdateResult.IsSuccess)
            {
                return userUpdateResult;
            }

            room.Roles.Add(role);
            var roomUpdateResult = await this.UpdateRoomRoles(room);
            if (!roomUpdateResult.IsSuccess)
            {
                return roomUpdateResult;
            }

            return new ExceptionalResult();
        }

        private async Task<OptionalResult<UserModel>> UpdateUserRoles(UserModel user)
        {
            var userUpdate = new UserUpdateModel()
            {
                Id = user.Id,
                Roles = user.Roles,
                IsActive = user.IsActive,
            };
            return await this.userService.Update(userUpdate);
        }

        private async Task<OptionalResult<RoomModel>> UpdateRoomRoles(RoomModel room)
        {
            var roomUpdate = new RoomUpdateModel()
            {
                Id = room.Id,
                Roles = room.Roles,
            };
            return await this.roomService.Update(roomUpdate);
        }

        private async Task<OptionalResult<RoleTypeModel>> GetRoleTypeByName(string name)
        {
            var result = (await this.roleTypeService.GetByCondition(x => x.Name == name)).FirstOrDefault();
            if (result is null)
            {
                return new OptionalResult<RoleTypeModel>(false, $"Role type with name {name} does not exist");
            }

            return new OptionalResult<RoleTypeModel>(result);
        }
    }
}
