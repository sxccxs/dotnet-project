using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;

namespace BLL.Services.UserServices
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService userService;

        private readonly IJwtService jwtService;

        public AuthenticationService(IUserService userService, IJwtService jwtService)
        {
            this.userService = userService;
            this.jwtService = jwtService;
        }

        public async Task<OptionalResult<UserModel>> GetUserByToken(string jwtToken)
        {
            var result = this.jwtService.ValidateJwt(jwtToken);
            var activeUsers = await this.userService.GetActiveUsers(x => x.Id == result.Value);
            if (!result.IsSuccess || !activeUsers.Any())
            {
                return new OptionalResult<UserModel>(false, "You are not logged in");
            }

            var user = await this.userService.GetUserById(result.Value);

            return new OptionalResult<UserModel>(user);
        }
    }
}
