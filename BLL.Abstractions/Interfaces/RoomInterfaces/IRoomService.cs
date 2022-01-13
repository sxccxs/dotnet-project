using Core.Models.RoomModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IRoomService
    {
        IEnumerable<RoomModel> GetAll();

        IEnumerable<RoomModel> GetByCondition(Func<RoomModel, bool> condition);

        RoomModel Create(RoomCreateModel roomModel);

        RoomModel Update(RoomUpdateModel roomModel);

        RoomModel Delete(int id);
    }
}
