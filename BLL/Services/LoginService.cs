using BLL.Abstractions.Interfaces;
using Core.Exceptions;
using Core.Models;

namespace BLL.Services
{
    internal class LoginService : ILoginService
    {
        private readonly IUserService userService;

        private readonly IJwtService jwtService;

        public LoginService(IUserService userService, IJwtService jwtService)
        {
            this.userService = userService;
            this.jwtService = jwtService;
        }

        public string Login(UserLoginModel userData)
        {
            var user = this.userService.GetByCondition(x => x.Email == userData.Email && x.IsActive).FirstOrDefault();
            if (user is null || this.userService.HashingService.Hash(userData.Password) != user.HashedPassword)
            {
                throw new UserDoesNotExistException($"User with given credantials does not exist.");
            }

            return this.jwtService.GenerateJwt(user);
        }
    }
}
