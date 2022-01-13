using AutoMapper;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.Exceptions;
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

        public IEnumerable<UserModel> Get()
        {
            return this.storage.GetAll();
        }

        public IEnumerable<UserModel> GetByCondition(Func<UserModel, bool> condition)
        {
            return this.storage.GetByCondition(condition);
        }

        public UserModel CreateNonActiveUser(UserCreateModel user)
        {
            if (this.storage.GetByCondition(x => x.Email == user.Email).Any())
            {
                throw new UserAlreadyExistsException($"User with email {user.Email} already exists");
            }

            var userModel = this.MapUserCreateModel(user);
            this.storage.Create(userModel);

            return userModel;
        }

        public UserModel Delete(int id)
        {
            var user = this.storage.GetByCondition(x => x.Id == id).FirstOrDefault();
            if (user is null)
            {
                throw new UserDoesNotExistException($"User with id {id} does not exist");
            }

            this.storage.Delete(user);

            return user;
        }

        public UserModel Update(UserUpdateModel user)
        {
            if (!this.storage.GetByCondition(x => x.Id == user.Id).Any())
            {
                throw new UserDoesNotExistException($"User with id {user.Id} does not exist");
            }

            var userModel = this.MapUserUpdateModel(user);
            this.storage.Update(userModel);

            return userModel;
        }

        public UserModel ActivateUser(int id)
        {
            if (!this.storage.GetByCondition(x => x.Id == id).Any())
            {
                throw new UserDoesNotExistException($"User with id {id} does not exist");
            }

            var userData = new UserUpdateModel()
            {
                Id = id,
                IsActive = true,
            };

            return this.Update(userData);
        }

        private UserModel MapUserCreateModel(UserCreateModel user)
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<UserCreateModel, UserModel>());
            var mapper = new Mapper(mapperConfig);
            var userObject = mapper.Map<UserModel>(user);
            userObject.Id = this.storage.GetNextId();
            userObject.HashedPassword = this.HashingService.Hash(user.Password);
            userObject.IsActive = false;

            return userObject;
        }

        private UserModel MapUserUpdateModel(UserUpdateModel user)
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<UserUpdateModel, UserModel>(MemberList.Source));
            var mapper = new Mapper(mapperConfig);
            var userObject = mapper.Map<UserModel>(user);
            var changingUser = this.GetByCondition(x => x.Id == user.Id).First();

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

            userObject.IsActive = changingUser.IsActive || user.IsActive;

            return userObject;
        }
    }
}
