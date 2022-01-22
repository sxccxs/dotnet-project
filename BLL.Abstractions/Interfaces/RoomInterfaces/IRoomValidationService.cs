using Core.DataClasses;
using Core.Models.RoomModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IRoomValidationService
    {
        ExceptionalResult ValidateCreateModel(RoomCreateModel createModel);

        ExceptionalResult ValidateUpdateModel(RoomUpdateModel updateModel);
    }
}
