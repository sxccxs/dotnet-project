using BLL.Abstractions.Interfaces.RoleInterfaces;
using Core.DataClasses;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.RoleServices
{
    internal class RoleService : IRoleService
    {
        private readonly IGenericStorageWorker<RoleModel> storageWorker;

        public RoleService(IGenericStorageWorker<RoleModel> storageWorker)
        {
            this.storageWorker = storageWorker;
        }

        public async Task<IEnumerable<RoleModel>> Get()
        {
            return await this.storageWorker.GetAll();
        }

        public async Task<IEnumerable<RoleModel>> GetByCondition(Func<RoleModel, bool> condition)
        {
            return (await this.Get()).Where(condition);
        }

        public async Task<OptionalResult<RoleModel>> CreateRole(RoleModel role)
        {
            role.Id = await this.storageWorker.GetNextId();
            await this.storageWorker.Create(role);

            return new OptionalResult<RoleModel>(role);
        }

        public async Task<OptionalResult<RoleModel>> Update(RoleModel role)
        {
            var changingRole = (await this.storageWorker.GetByCondition(x => x.Id == role.Id)).FirstOrDefault();
            if (changingRole is null)
            {
                return new OptionalResult<RoleModel>(false, $"Role with id {role.Id} does not exist.");
            }

            changingRole.Role = role.Role;
            await this.storageWorker.Update(changingRole);

            return new OptionalResult<RoleModel>(changingRole);
        }

        public async Task<OptionalResult<RoleModel>> Delete(int id)
        {
            var role = (await this.storageWorker.GetByCondition(x => x.Id == id)).FirstOrDefault();
            if (role is null)
            {
                return new OptionalResult<RoleModel>(false, $"Role with id {id} does not exist.");
            }

            await this.storageWorker.Delete(role);

            return new OptionalResult<RoleModel>(role);
        }
    }
}
