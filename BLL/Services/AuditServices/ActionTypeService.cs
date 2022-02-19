using System.Linq.Expressions;
using BLL.Abstractions.Interfaces.AuditInterfaces;
using Core.DataClasses;
using Core.Models.AuditModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.AuditServices;

public class ActionTypeService : IActionTypeService
{
    private readonly IGenericStorageWorker<ActionTypeModel> storage;

    public ActionTypeService(IGenericStorageWorker<ActionTypeModel> storage)
    {
        this.storage = storage;
    }

    public async Task<OptionalResult<ActionTypeModel>> CreateRole(ActionTypeModel actionType)
    {
        await this.storage.Create(actionType);
        var result = (await this.GetByCondition(x => x.Name == actionType.Name)).First();

        return new OptionalResult<ActionTypeModel>(result);
    }

    public async Task<IEnumerable<ActionTypeModel>> GetByCondition(Expression<Func<ActionTypeModel, bool>> condition)
    {
        return await this.storage.GetByConditions(new[] { condition });
    }
}