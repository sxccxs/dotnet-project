using Core.DataClasses;
using Core.Enums;
using Core.Models;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.RoleInterfaces
{
    public interface IUserRoomRoleService
    {
        Task<IEnumerable<RoleModel>> GetRolesForUser(UserModel user);

        Task<IEnumerable<RoleModel>> GetRolesForRoom(RoomModel room);

        Task<RoleModel> GetRoleForUserAndRoom(UserModel user, RoomModel room);

        Task<ExceptionalResult> AddRoleForUserAndRoom(UserModel user, RoomModel room, Role roleType);

        Task<OptionalResult<RoleModel>> DeleteRoleForUserAndRoom(UserModel user, RoomModel room);

        Task<ExceptionalResult> UpdateRoleForUser(UserModel user, RoomModel room, Role role);

        Task<ExceptionalResult> CheckOneAdminInRoom(UserModel user, RoomModel room);
    }
}
