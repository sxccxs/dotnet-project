using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        IHashingService HashingService { get; }

        Task<OptionalResult<UserModel>> CreateNonActiveUser(UserCreateModel user);

        Task<OptionalResult<UserModel>> ActivateUser(int id);

        Task<OptionalResult<UserModel>> Delete(int id);

        Task<OptionalResult<UserModel>> Update(UserUpdateModel user);

        Task<IEnumerable<UserModel>> Get();

        Task<IEnumerable<UserModel>> GetByCondition(Func<UserModel, bool> condition);
    }
}
