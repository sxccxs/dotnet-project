using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IValidationService
    {
        void Validate(UserRegistrationModel user);
    }
}
