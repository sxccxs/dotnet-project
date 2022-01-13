using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IUserValidationService
    {
        void Validate(UserRegistrationModel user);
    }
}
