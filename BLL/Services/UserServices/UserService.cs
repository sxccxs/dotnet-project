using AutoMapper;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.UserServices
{
    internal class UserService : IUserService
    {
        private readonly IGenericStorageWorker<UserModel> storage;

        public UserService(IGenericStorageWorker<UserModel> storage, IHashingService hashingService)
        {
            this.storage = storage;
            this.HashingService = hashingService;
        }

        public IHashingService HashingService { get; private set; }

        public async Task<IEnumerable<UserModel>> GetByCondition(Func<UserModel, bool> condition)
        {
            return await this.storage.GetByCondition(condition);
        }

        public async Task<IEnumerable<UserModel>> GetActiveUsers(Func<UserModel, bool> additionalCondition)
        {
            return await this.GetByCondition(x => x.IsActive && additionalCondition(x));
        }

        public async Task<OptionalResult<UserModel>> CreateNonActiveUser(UserCreateModel user)
        {
            if ((await this.storage.GetByCondition(x => x.Email == user.Email)).Any())
            {
                return new OptionalResult<UserModel>(false, $"User with email {user.Email} already exists");
            }

            var userModel = await this.MapUserCreateModel(user);
            await this.storage.Create(userModel);

            return new OptionalResult<UserModel>(userModel);
        }

        public async Task<OptionalResult<UserModel>> Delete(int id)
        {
            var user = (await this.storage.GetByCondition(x => x.Id == id)).FirstOrDefault();
            if (user is null)
            {
                return new OptionalResult<UserModel>(false, $"User with id {id} does not exist");
            }

            await this.storage.Delete(user);

            return new OptionalResult<UserModel>(user);
        }

        public async Task<OptionalResult<UserModel>> Update(UserUpdateModel user)
        {
            if (!(await this.storage.GetByCondition(x => x.Id == user.Id)).Any())
            {
                return new OptionalResult<UserModel>(false, $"User with id {user.Id} does not exist");
            }

            var userModel = await this.MapUserUpdateModel(user);
            await this.storage.Update(userModel);

            return new OptionalResult<UserModel>(userModel);
        }

        public async Task<OptionalResult<UserModel>> ActivateUser(int id)
        {
            var userData = new UserUpdateModel()
            {
                Id = id,
                IsActive = true,
            };

            return await this.Update(userData);
        }

        private async Task<UserModel> MapUserCreateModel(UserCreateModel user)
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<UserCreateModel, UserModel>());
            var mapper = new Mapper(mapperConfig);
            var userObject = mapper.Map<UserModel>(user);
            userObject.Id = await this.storage.GetNextId();
            userObject.HashedPassword = this.HashingService.Hash(user.Password);
            userObject.IsActive = false;

            return userObject;
        }

        private async Task<UserModel> MapUserUpdateModel(UserUpdateModel user)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserUpdateModel, UserModel>(MemberList.Source).ForAllMembers(opt => opt.AllowNull());
            });
            var mapper = new Mapper(mapperConfig);
            var userObject = mapper.Map<UserModel>(user);
            var changingUser = (await this.GetByCondition(x => x.Id == user.Id)).First();

            foreach (var field in userObject.GetType().GetProperties())
            {
                if (field.GetValue(userObject) is null)
                {
                    field.SetValue(userObject, field.GetValue(changingUser));
                }
            }

            if (user.Password is not null)
            {
                userObject.HashedPassword = this.HashingService.Hash(user.Password);
            }

            return userObject;
        }
    }
}
