using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IRegistrationService
    {
        Task<ExceptionalResult> Register(UserRegistrationModel userData);
    }
}
