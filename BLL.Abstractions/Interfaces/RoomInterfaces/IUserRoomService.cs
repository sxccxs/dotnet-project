using Core.DataClasses;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IUserRoomService
    {
        Task<ExceptionalResult> CreateRoomForUser(UserModel user, RoomCreateModel createModel, bool asTransaction = true);

        Task<ExceptionalResult> UpdateRoomForUser(UserModel user, RoomEditModel editModel, bool asTransaction = true);

        Task<ExceptionalResult> DeleteRoomByUser(UserModel user, int roomId, bool asTransaction = true);

        Task<ExceptionalResult> AddUserToRoom(string email, RoomModel room,  UserModel actor, bool asTransaction = true);

        Task<ExceptionalResult> DeleteUserFromRoom(UserModel user, RoomModel room, UserModel userToDelete, bool asTransaction = true);

        Task<ExceptionalResult> DeleteUserAndRooms(UserModel user, bool asTransaction = true);
    }
}
