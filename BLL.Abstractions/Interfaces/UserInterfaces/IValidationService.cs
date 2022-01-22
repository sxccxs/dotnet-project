using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IValidationService
    {
        ExceptionalResult Validate(UserRegistrationModel user);
    }
}
