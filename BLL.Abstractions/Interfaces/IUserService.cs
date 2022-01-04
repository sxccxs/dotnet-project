using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        void Create(UserCreateModel user);
        void Delete(int id);
        void Update(UserUpdateModel user);
        IEnumerable<UserModel> Get();
        IEnumerable<UserModel> GetByCondition(Func<UserModel, bool> condition);
    }
}
