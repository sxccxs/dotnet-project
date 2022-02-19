using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models.AuditModels;

namespace BLL.Abstractions.Interfaces.AuditInterfaces;

public interface IActionTypeService
{
    Task<OptionalResult<ActionTypeModel>> CreateRole(ActionTypeModel actionType);

    Task<IEnumerable<ActionTypeModel>> GetByCondition(Expression<Func<ActionTypeModel, bool>> condition);
}