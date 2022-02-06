using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IDeleteUserService
    {
        Task<OptionalResult<string>> DeleteUser(OptionalResult<int> userId);
    }
}
