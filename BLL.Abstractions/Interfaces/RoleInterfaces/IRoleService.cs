using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models;
using Core.Models.RoleModels;

namespace BLL.Abstractions.Interfaces.RoleInterfaces
{
    public interface IRoleService
    {
        Task<OptionalResult<RoleModel>> CreateRole(RoleModel role);

        Task<ExceptionalResult> Delete(int id);

        Task<OptionalResult<RoleModel>> Update(RoleModel role);

        Task<IEnumerable<RoleModel>> GetByConditions(params Expression<Func<RoleModel, bool>>[] conditions);
    }
}
