using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        Task<OptionalResult<UserModel>> CreateNonActiveUser(UserCreateModel user);

        Task<OptionalResult<UserModel>> ActivateUser(int id);

        Task<ExceptionalResult> Delete(int id);

        Task<OptionalResult<UserModel>> Update(UserUpdateModel user);

        Task<IEnumerable<UserModel>> GetByConditions(params Expression<Func<UserModel, bool>>[] conditions);

        Task<IEnumerable<UserModel>> GetActiveUsers(params Expression<Func<UserModel, bool>>[] additionalConditions);

        Task<UserModel> GetUserById(int id);
    }
}
