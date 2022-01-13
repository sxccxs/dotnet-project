using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface ILoginService
    {
        string Login(UserLoginModel userData);
    }
}
