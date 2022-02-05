using System.Linq.Expressions;
using BLL.Abstractions.Interfaces.RoleInterfaces;
using Core.DataClasses;
using Core.Models;
using Core.Models.UserModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.RoleServices;

public class RoleTypeService : IRoleTypeService
{
    private readonly IGenericStorageWorker<RoleTypeModel> storage;

    public RoleTypeService(IGenericStorageWorker<RoleTypeModel> storage)
    {
        this.storage = storage;
    }

    public async Task<OptionalResult<RoleTypeModel>> CreateRole(RoleTypeModel roleType)
    {
        await this.storage.Create(roleType);
        var result = (await this.GetByCondition(x => x.Name == roleType.Name)).First();

        return new OptionalResult<RoleTypeModel>(result);
    }

    public async Task<IEnumerable<RoleTypeModel>> GetByCondition(Expression<Func<RoleTypeModel, bool>> condition)
    {
        return await this.storage.GetByConditions(new[] { condition });
    }
}