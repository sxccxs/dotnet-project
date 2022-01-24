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
            if (!result.IsSuccess || !(await this.userService.GetByCondition(x => x.Id == result.Value)).Any())
            {
                return new OptionalResult<UserModel>(false, "You are not logged in");
            }

            var user = (await this.userService.GetByCondition(x => x.Id == result.Value)).First();

            return new OptionalResult<UserModel>(user);
        }
    }
}
