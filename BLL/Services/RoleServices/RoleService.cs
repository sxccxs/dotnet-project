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

        public async Task<IEnumerable<RoleModel>> GetByCondition(Func<RoleModel, bool> condition)
        {
            return await this.storage.GetByCondition(condition);
        }

        public async Task<OptionalResult<RoleModel>> CreateRole(RoleModel role)
        {
            role.Id = await this.storage.GetNextId();
            await this.storage.Create(role);

            return new OptionalResult<RoleModel>(role);
        }

        public async Task<OptionalResult<RoleModel>> Update(RoleModel role)
        {
            var changingRole = (await this.storage.GetByCondition(x => x.Id == role.Id)).FirstOrDefault();
            if (changingRole is null)
            {
                return new OptionalResult<RoleModel>(false, $"Role with id {role.Id} does not exist.");
            }

            changingRole.Role = role.Role;
            await this.storage.Update(changingRole);

            return new OptionalResult<RoleModel>(changingRole);
        }

        public async Task<OptionalResult<RoleModel>> Delete(int id)
        {
            var role = (await this.storage.GetByCondition(x => x.Id == id)).FirstOrDefault();
            if (role is null)
            {
                return new OptionalResult<RoleModel>(false, $"Role with id {id} does not exist.");
            }

            await this.storage.Delete(role);

            return new OptionalResult<RoleModel>(role);
        }
    }
}
