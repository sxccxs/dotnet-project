using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IUserRoomService
    {
        void CreateRoomForUser(UserModel user, RoomCreateModel createModel);

        void UpdateRoomForUser(UserModel user, RoomUpdateModel updateModel);

        void DeleteRoomByUser(UserModel user, int roomId);

        IEnumerable<RoomModel> GetRoomsForUser(UserModel user);

        IEnumerable<UserModel> GetUsersInRoom(RoomModel room);
    }
}
