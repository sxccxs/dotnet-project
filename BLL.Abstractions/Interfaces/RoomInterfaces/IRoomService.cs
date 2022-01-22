using Core.DataClasses;
using Core.Models.RoomModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IRoomService
    {
        IEnumerable<RoomModel> GetAll();

        IEnumerable<RoomModel> GetByCondition(Func<RoomModel, bool> condition);

        OptionalResult<RoomModel> Create(RoomCreateModel roomModel);

        OptionalResult<RoomModel> Update(RoomUpdateModel roomModel);

        OptionalResult<RoomModel> Delete(int id);
    }
}
