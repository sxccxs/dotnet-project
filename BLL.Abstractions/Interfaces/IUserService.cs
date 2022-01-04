using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        void Create(UserModel user);
        void Delete(UserModel user);
        void Update(UserModel user);
        IEnumerable<UserModel> Get();

        IEnumerable<UserModel> GetByCondition(Func<UserModel, bool> condition);
    }
}
