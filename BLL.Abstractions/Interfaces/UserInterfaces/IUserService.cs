using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        Task<OptionalResult<UserModel>> CreateNonActiveUser(UserCreateModel user);

        Task<OptionalResult<UserModel>> ActivateUser(int id);

        Task<OptionalResult<UserModel>> Delete(int id);

        Task<OptionalResult<UserModel>> Update(UserUpdateModel user);

        Task<IEnumerable<UserModel>> GetByCondition(Func<UserModel, bool> condition);

        Task<IEnumerable<UserModel>> GetActiveUsers(Func<UserModel, bool> additionalCondition);

        Task<UserModel> GetUserById(int id);
    }
}
