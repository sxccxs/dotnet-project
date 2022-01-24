using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IAuthenticationService
    {
        Task<OptionalResult<UserModel>> GetUserByToken(string jwtToken);
    }
}
