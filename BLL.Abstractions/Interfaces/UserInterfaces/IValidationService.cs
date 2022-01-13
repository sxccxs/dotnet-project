using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IValidationService
    {
        void Validate(UserRegistrationModel user);
    }
}
