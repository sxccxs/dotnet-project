using Core.DataClasses;
using Core.Models;

namespace BLL.Abstractions.Interfaces.RoleInterfaces
{
    public interface IRoleService
    {
        Task<OptionalResult<RoleModel>> CreateRole(RoleModel role);

        Task<OptionalResult<RoleModel>> Delete(int id);

        Task<OptionalResult<RoleModel>> Update(RoleModel role);

        Task<IEnumerable<RoleModel>> Get();

        Task<IEnumerable<RoleModel>> GetByCondition(Func<RoleModel, bool> condition);
    }
}
