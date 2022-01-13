using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.Exceptions;
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

        public UserModel GetUserByToken(string jwtToken)
        {
            var result = this.jwtService.ValidateJwt(jwtToken);
            if (!result.HasValue || !this.userService.GetByCondition(x => x.Id == result.Value).Any())
            {
                throw new AuthorizationException("You are not logged in");
            }

            var user = this.userService.GetByCondition(x => x.Id == result.Value).First();

            return user;
        }
    }
}
