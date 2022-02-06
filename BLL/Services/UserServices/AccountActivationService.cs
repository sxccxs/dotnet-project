using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;

namespace BLL.Services.UserServices
{
    public class AccountActivationService : IAccountActivationService
    {
        private readonly ITokenGeneratorService tokenGeneratorService;

        private readonly IUserService userService;

        public AccountActivationService(ITokenGeneratorService tokenGeneratorService, IUserService userService)
        {
            this.tokenGeneratorService = tokenGeneratorService;
            this.userService = userService;
        }

        public async Task<ExceptionalResult> Activate(AccountActivationPayload activationPayload)
        {
            var idResult = this.tokenGeneratorService.GetIdFromUidb64(activationPayload.Uidb64);
            if (!idResult.IsSuccess)
            {
                return idResult;
            }

            var id = idResult.Value;
            var user = await this.userService.GetUserById(id);
            if (user is null)
            {
                return new ExceptionalResult(false, $"User with id {id} does not exist");
            }

            var tokenResult = this.tokenGeneratorService.CheckToken(user, activationPayload.Token);

            if (!tokenResult.IsSuccess && tokenResult.ExceptionMessage == "Token is outdated")
            {
                await this.userService.Delete(id);
                return new ExceptionalResult(false, "Token is outdated. You have to register again");
            }

            if (!tokenResult.IsSuccess)
            {
                return tokenResult;
            }

            await this.userService.ActivateUser(id);
            return new ExceptionalResult();
        }
    }
}
