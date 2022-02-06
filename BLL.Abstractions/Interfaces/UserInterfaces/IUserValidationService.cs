using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IUserValidationService
    {
        ExceptionalResult Validate(UserRegistrationModel user);

        ExceptionalResult ValidateUpdateModel(UserEditModel user);
    }
}
