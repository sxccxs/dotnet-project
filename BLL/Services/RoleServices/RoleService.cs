using System.Linq.Expressions;
using BLL.Abstractions.Interfaces.RoleInterfaces;
using Core.DataClasses;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.RoleServices
{
    internal class RoleService : IRoleService
    {
        private readonly IGenericStorageWorker<RoleModel> storage;

        public RoleService(IGenericStorageWorker<RoleModel> storage)
        {
            this.storage = storage;
        }

        public async Task<IEnumerable<RoleModel>> GetByConditions(params Expression<Func<RoleModel, bool>>[] conditions)
        {
            return await this.storage.GetByConditions(conditions, r => r.Room, r => r.User, r => r.RoleType);
        }

        public async Task<RoleModel> GetRoleById(int id)
        {
            return (await this.GetByConditions(r => r.Id == id)).FirstOrDefault();
        }

        public async Task<OptionalResult<RoleModel>> CreateRole(RoleModel role)
        {
            await this.storage.Create(role);

            return new OptionalResult<RoleModel>(role);
        }

        public async Task<OptionalResult<RoleModel>> Update(RoleModel role)
        {
            var changingRole = await this.GetRoleById(role.Id);
            if (changingRole is null)
            {
                return new OptionalResult<RoleModel>(false, $"Role with id {role.Id} does not exist.");
            }

            changingRole.RoleType = role.RoleType;
            await this.storage.Update(changingRole);

            return new OptionalResult<RoleModel>(role);
        }

        public async Task<ExceptionalResult> Delete(int id)
        {
            var role = await this.GetRoleById(id);
            if (role is null)
            {
                return new ExceptionalResult(false, $"Role with id {id} does not exist.");
            }

            await this.storage.Delete(role);

            return new ExceptionalResult();
        }
    }
}
