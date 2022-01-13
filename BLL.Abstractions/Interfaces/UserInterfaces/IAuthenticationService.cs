using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IAuthenticationService
    {
        UserModel GetUserByToken(string jwtToken);
    }
}
