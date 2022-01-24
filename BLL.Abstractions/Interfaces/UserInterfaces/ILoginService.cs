using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface ILoginService
    {
        Task<OptionalResult<string>> Login(UserLoginModel userData);
    }
}
