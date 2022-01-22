using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        IHashingService HashingService { get; }

        OptionalResult<UserModel> CreateNonActiveUser(UserCreateModel user);

        OptionalResult<UserModel> ActivateUser(int id);

        OptionalResult<UserModel> Delete(int id);

        OptionalResult<UserModel> Update(UserUpdateModel user);

        IEnumerable<UserModel> Get();

        IEnumerable<UserModel> GetByCondition(Func<UserModel, bool> condition);
    }
}
