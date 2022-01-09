using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface ILoginService
    {
        string Login(UserLoginModel userData);
    }
}
