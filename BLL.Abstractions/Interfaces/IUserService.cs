using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        IHashingService HashingService { get; }

        UserModel CreateNonActiveUser(UserCreateModel user);

        UserModel ActivateUser(int id);

        UserModel Delete(int id);

        UserModel Update(UserUpdateModel user);

        IEnumerable<UserModel> Get();

        IEnumerable<UserModel> GetByCondition(Func<UserModel, bool> condition);
    }
}
