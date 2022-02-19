using BLL.Abstractions.Interfaces;
using BLL.Abstractions.Interfaces.AuditInterfaces;
using BLL.Abstractions.Interfaces.RoleInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.AuditModels;
using Core.Models.RoleModels;

namespace BLL.Services;

public class StartupService : IStartupService
{
    private readonly IRoleTypeService roleTypeService;

    private readonly IActionTypeService actionTypeService;

    public StartupService(IRoleTypeService roleTypeService, IActionTypeService actionTypeService)
    {
        this.roleTypeService = roleTypeService;
        this.actionTypeService = actionTypeService;
    }

    public async Task<ExceptionalResult> SetUp()
    {
        var results = new[]
        {
            await this.SetUpRoleTypes(),
            await this.SetUpActionTypes(),
        };

        return results.FirstOrDefault(r => !r.IsSuccess) ?? new ExceptionalResult();
    }

    private async Task<ExceptionalResult> SetUpRoleTypes()
    {
        foreach (var name in Enum.GetNames(typeof(RoleType)))
        {
            var roleType = (await this.roleTypeService.GetByCondition(rt => rt.Name == name)).FirstOrDefault();
            if (roleType is not null)
            {
                continue;
            }

            var roleT = new RoleTypeModel() { Name = name };
            var result = await this.roleTypeService.CreateRole(roleT);
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return new ExceptionalResult();
    }

    private async Task<ExceptionalResult> SetUpActionTypes()
    {
        foreach (var name in Enum.GetNames(typeof(ActionType)))
        {
            var actionType = (await this.actionTypeService.GetByCondition(rt => rt.Name == name)).FirstOrDefault();
            if (actionType is not null)
            {
                continue;
            }

            var actionT = new ActionTypeModel() { Name = name };
            var result = await this.actionTypeService.CreateRole(actionT);
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return new ExceptionalResult();
    }
}