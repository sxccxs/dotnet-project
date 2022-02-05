using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Services.UserServices
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

        public async Task<OptionalResult<string>> Login(UserLoginModel userData)
        {
            var user = (await this.userService.GetActiveUsers(x => x.Email == userData.Email)).FirstOrDefault();
            if (user is null || this.userService.HashingService.Hash(userData.Password) != user.HashedPassword)
            {
                return new OptionalResult<string>(false, $"User with given credentials does not exist.");
            }

            return new OptionalResult<string>(this.jwtService.GenerateJwt(user));
        }
    }
}
