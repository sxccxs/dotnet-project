using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models;
using Core.Models.RoleModels;
using Core.Models.UserModels;

namespace BLL.Abstractions.Interfaces.RoleInterfaces;

public interface IRoleTypeService
{
    Task<OptionalResult<RoleTypeModel>> CreateRole(RoleTypeModel roleType);

    Task<IEnumerable<RoleTypeModel>> GetByCondition(Expression<Func<RoleTypeModel, bool>> condition);
}