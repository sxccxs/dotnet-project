using Core.DataClasses;
using Core.Enums;
using Core.Models;
using Core.Models.RoleModels;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.RoleInterfaces
{
    public interface IUserRoomRoleService
    {
        Task<RoleModel> GetRoleForUserAndRoom(UserModel user, RoomModel room);

        Task<ExceptionalResult> AddRoleForUserAndRoom(UserModel user, RoomModel room, string roleName, bool asTransaction = true);

        Task<ExceptionalResult> UpdateRoleForUser(UserModel user, RoomModel room, string roleName, UserModel actor, bool asTransaction = true);

        Task<bool> IsUserLastAdminInRoom(UserModel user, RoomModel room);
    }
}
