using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        IHashingService HashingService { get; }

        void CreateNonActiveUser(UserCreateModel user);

        void ActivateUser(int id);

        void Delete(int id);

        void Update(UserUpdateModel user);

        IEnumerable<UserModel> Get();

        IEnumerable<UserModel> GetByCondition(Func<UserModel, bool> condition);
    }
}
