using Core.DataClasses;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IUserRoomService
    {
        ExceptionalResult CreateRoomForUser(UserModel user, RoomCreateModel createModel);

        ExceptionalResult UpdateRoomForUser(UserModel user, RoomUpdateModel updateModel);

        ExceptionalResult DeleteRoomByUser(UserModel user, int roomId);

        IEnumerable<RoomModel> GetRoomsForUser(UserModel user);

        IEnumerable<UserModel> GetUsersInRoom(RoomModel room);
    }
}
