using Core.Models.RoomModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IRoomValidationService
    {
        void ValidateCreateModel(RoomCreateModel createModel);

        void ValidateUpdateModel(RoomUpdateModel updateModel);
    }
}
