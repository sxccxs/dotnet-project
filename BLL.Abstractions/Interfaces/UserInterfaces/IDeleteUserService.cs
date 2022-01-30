using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IDeleteUserService
    {
        Task<ExceptionalResult> DeleteUser(UserModel user);
    }
}
