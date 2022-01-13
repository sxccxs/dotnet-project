using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IRegistrationService
    {
        void Register(UserRegistrationModel userData);
    }
}
