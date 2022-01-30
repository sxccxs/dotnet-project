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

        public UserRoomRoleService(IRoomService roomService, IUserService userService, IRoleService roleService)
        {
            this.roomService = roomService;
            this.userService = userService;
            this.roleService = roleService;
        }

        public async Task<IEnumerable<RoleModel>> GetRolesForUser(UserModel user)
        {
            var tasks = await Task.WhenAll(user.Roles.Select(id => this.roleService.GetByCondition(x => x.Id == id)));
            return tasks.Where(result => result is not null).Select(result => result.First());
        }

        public async Task<IEnumerable<RoleModel>> GetRolesForRoom(RoomModel room)
        {
            var tasks = await Task.WhenAll(room.Roles.Select(id => this.roleService.GetByCondition(x => x.Id == id)));
            return tasks.Where(result => result is not null).Select(result => result.First());
        }

        public async Task<RoleModel> GetRoleForUserAndRoom(UserModel user, RoomModel room)
        {
            return (await this.GetRolesForUser(user)).Where(x => x.RoomId == room.Id).FirstOrDefault();
        }

        public async Task<ExceptionalResult> AddRoleForUserAndRoom(UserModel user, RoomModel room, Role roleType)
        {
            var role = new RoleModel()
            {
                Role = roleType,
                UserId = user.Id,
                RoomId = room.Id,
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

            user.Roles.Remove(role.Id);
            var userResult = await this.UpdateUserRoles(user);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<RoleModel>(userResult);
            }

            room.Roles.Remove(role.Id);
            var roomResult = await this.UpdateRoomRoles(room);
            if (!roomResult.IsSuccess)
            {
                return new OptionalResult<RoleModel>(roomResult);
            }

            var roleResult = await this.roleService.Delete(role.Id);
            if (!roleResult.IsSuccess)
            {
                return roleResult;
            }

            return new OptionalResult<RoleModel>(role);
        }

        public async Task<ExceptionalResult> UpdateRoleForUser(UserModel user, RoomModel room, Role role)
        {
            var userRole = await this.GetRoleForUserAndRoom(user, room);
            if (userRole is null)
            {
                return new ExceptionalResult(false, $"User or room does not exist or user {user.Id} does not belong to room {room.Id}");
            }

            if (role == Role.MEMBER)
            {
                var roomRoles = await this.GetRolesForRoom(room);
                if (!roomRoles.Any(x => x.Role == Role.ADMIN && x.UserId != user.Id))
                {
                    return new ExceptionalResult(false, "Room must have at least one admin. Make someone else admin first");
                }
            }

            userRole.Role = role;
            await this.roleService.Update(userRole);

            return new ExceptionalResult();
        }

        private async Task<ExceptionalResult> AddRoleModelToUserAndRoom(UserModel user, RoomModel room, RoleModel role)
        {
            user.Roles.Add(role.Id);
            var userUpdateResult = await this.UpdateUserRoles(user);
            if (!userUpdateResult.IsSuccess)
            {
                return userUpdateResult;
            }

            room.Roles.Add(role.Id);
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
    }
}
