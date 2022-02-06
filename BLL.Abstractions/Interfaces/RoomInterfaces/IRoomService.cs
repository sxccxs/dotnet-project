using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models.RoomModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomModel>> GetByConditions(params Expression<Func<RoomModel, bool>>[] conditions);

        Task<RoomModel> GetRoomById(int id);

        Task<OptionalResult<RoomModel>> Create(RoomCreateModel roomModel);

        Task<OptionalResult<RoomModel>> Update(RoomUpdateModel roomModel);

        Task<ExceptionalResult> Delete(int id);
    }
}
