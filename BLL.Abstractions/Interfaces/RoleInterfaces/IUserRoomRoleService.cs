using Core.DataClasses;
using Core.Enums;
using Core.Models;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.RoleInterfaces
{
    public interface IUserRoomRoleService
    {
        Task<RoleModel> GetRoleForUserAndRoom(UserModel user, RoomModel room);

        Task<ExceptionalResult> AddRoleForUserAndRoom(UserModel user, RoomModel room, string roleName);

        Task<OptionalResult<RoleModel>> DeleteRoleForUserAndRoom(UserModel user, RoomModel room);

        Task<ExceptionalResult> UpdateRoleForUser(UserModel user, RoomModel room, string roleName);

        Task<bool> IsUserLastAdminInRoom(UserModel user, RoomModel room);
    }
}
