using BLL.Abstractions.Interfaces.AuditInterfaces;
using BLL.Abstractions.Interfaces.RoleInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.AuditModels;
using Core.Models.RoleModels;
using Core.Models.RoomModels;
using Core.Models.UserModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.RoleServices
{
    internal class UserRoomRoleService : IUserRoomRoleService
    {
        private readonly IRoleService roleService;

        private readonly IRoleTypeService roleTypeService;

        private readonly IAuditService auditService;

        private readonly ITransactionsWorker transactionsWorker;

        public UserRoomRoleService(
            IAuditService auditService,
            IRoleService roleService,
            IRoleTypeService roleTypeService,
            ITransactionsWorker transactionsWorker)
        {
            this.auditService = auditService;
            this.transactionsWorker = transactionsWorker;
            this.roleService = roleService;
            this.roleTypeService = roleTypeService;
        }

        public async Task<RoleModel> GetRoleForUserAndRoom(UserModel user, RoomModel room)
        {
            return (await this.roleService.GetByConditions(r => r.User == user, r => r.Room == room)).FirstOrDefault();
        }

        public async Task<ExceptionalResult> AddRoleForUserAndRoom(
            UserModel user,
            RoomModel room,
            string roleName,
            bool asTransaction = true)
        {
            return asTransaction
                ? await this.transactionsWorker.RunAsTransaction(() => this.InnerAddRoleForUserAndRoom(user, room, roleName))
                : await this.InnerAddRoleForUserAndRoom(user, room, roleName);
        }

        public async Task<bool> IsUserLastAdminInRoom(UserModel user, RoomModel room)
        {
            return !(await this.roleService.GetByConditions(r => r.Room == room)).Any(r =>
                r.RoleType.Name == RoleType.Admin.ToString() && r.User != user);
        }

        public async Task<ExceptionalResult> UpdateRoleForUser(UserModel user, RoomModel room, string roleName, UserModel actor)
        {
            return await this.transactionsWorker.RunAsTransaction(() =>
                this.InnerUpdateRoleForUser(user, room, roleName, actor));
        }

        private async Task<ExceptionalResult> InnerUpdateRoleForUser(UserModel user, RoomModel room, string roleName, UserModel actor)
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

            var oldRole = userRole.RoleType;

            if (roleType.Name != RoleType.Admin.ToString())
            {
                if (await this.IsUserLastAdminInRoom(user, room))
                {
                    return new ExceptionalResult(false, "Room must have at least one admin. Make someone else admin first");
                }
            }

            userRole.RoleType = roleType;
            var updateResult = await this.roleService.Update(userRole);
            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            var record = new CreateAuditRecordModel()
            {
                ActionType = ActionType.ChangeUserRoleType,
                Actor = actor,
                Room = room,
                UserUnderAction = user,
                OldRole = oldRole,
                NewRole = roleType,
            };

            return await this.auditService.CreateAuditRecord(record);
        }

        private async Task<ExceptionalResult> InnerAddRoleForUserAndRoom(UserModel user, RoomModel room, string roleName)
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

            return await this.roleService.CreateRole(role);
        }

        private async Task<OptionalResult<RoleTypeModel>> GetRoleTypeByName(string name)
        {
            var result = (await this.roleTypeService.GetByCondition(x => x.Name == name)).FirstOrDefault();
            return result is null ? new OptionalResult<RoleTypeModel>(false, $"Role type with name {name} does not exist") : new OptionalResult<RoleTypeModel>(result);
        }
    }
}
