using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IRegistrationService
    {
        ExceptionalResult Register(UserRegistrationModel userData);
    }
}
