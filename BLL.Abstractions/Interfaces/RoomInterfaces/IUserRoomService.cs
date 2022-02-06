using Core.DataClasses;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IUserRoomService
    {
        Task<ExceptionalResult> CreateRoomForUser(UserModel user, RoomCreateModel createModel);

        Task<ExceptionalResult> UpdateRoomForUser(UserModel user, RoomEditModel editModel);

        Task<ExceptionalResult> DeleteRoomByUser(UserModel user, int roomId);

        Task<IEnumerable<RoomModel>> GetRoomsForUser(UserModel user);

        Task<IEnumerable<UserModel>> GetUsersInRoom(RoomModel room);

        Task<ExceptionalResult> AddUserToRoom(string email, RoomModel room);

        Task<ExceptionalResult> DeleteUserFromRoom(UserModel user, RoomModel room);
    }
}
