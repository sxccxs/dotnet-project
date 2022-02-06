using System.Linq.Expressions;
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

        private readonly IHashingService hashingService;

        public UserService(IGenericStorageWorker<UserModel> storage, IHashingService hashingService)
        {
            this.storage = storage;
            this.hashingService = hashingService;
        }

        public async Task<IEnumerable<UserModel>> GetByConditions(params Expression<Func<UserModel, bool>>[] conditions)
        {
            return await this.storage.GetByConditions(conditions, u => u.Rooms, u => u.Roles);
        }

        public async Task<IEnumerable<UserModel>> GetActiveUsers(params Expression<Func<UserModel, bool>>[] additionalConditions)
        {
            additionalConditions = additionalConditions.Append(x => x.IsActive).ToArray();
            return await this.GetByConditions(additionalConditions);
        }

        public async Task<UserModel> GetUserById(int id)
        {
            return (await this.GetByCondition(u => u.Id == id)).FirstOrDefault();
        }

        public async Task<OptionalResult<UserModel>> CreateNonActiveUser(UserCreateModel user)
        {
            if ((await this.GetByConditions(x => x.Email == user.Email)).Any())
            {
                return new OptionalResult<UserModel>(false, $"User with email {user.Email} already exists");
            }

            var userModel = this.MapUserCreateModel(user);
            await this.storage.Create(userModel);

            return new OptionalResult<UserModel>(userModel);
        }

        public async Task<ExceptionalResult> Delete(int id)
        {
            var user = await this.GetUserById(id);
            if (user is null)
            {
                return new ExceptionalResult(false, $"User with id {id} does not exist");
            }

            await this.storage.Delete(user);

            return new ExceptionalResult();
        }

        public async Task<OptionalResult<UserModel>> Update(UserUpdateModel user)
        {
            if (await this.GetUserById(user.Id) is null)
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

        private UserModel MapUserCreateModel(UserCreateModel user)
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<UserCreateModel, UserModel>());
            var mapper = new Mapper(mapperConfiguration);
            var userObject = mapper.Map<UserModel>(user);
            userObject.HashedPassword = this.hashingService.Hash(user.Password);
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
            var changingUser = (await this.GetByConditions(x => x.Id == user.Id)).First();

            foreach (var field in userObject.GetType().GetProperties())
            {
                if (field.GetValue(userObject) is not null)
                {
                    field.SetValue(changingUser, field.GetValue(userObject));
                }
            }

            if (user.Password is not null)
            {
                changingUser.HashedPassword = this.HashingService.Hash(user.Password);
            }

            return changingUser;
        }
    }
}
