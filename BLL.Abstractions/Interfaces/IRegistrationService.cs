using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IRegistrationService
    {
        void Register(UserRegistrationModel userData);
    }
}
