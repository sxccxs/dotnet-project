using BLL.Abstractions.Interfaces;
using Core.DataClasses;
using Core.Exceptions;

namespace BLL.Services
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

        public void Activate(AccountActivationPayload activationPayload)
        {
            var id = this.tokenGeneratorService.GetIdFromUidb64(activationPayload.Uidb64);
            var user = this.userService.GetByCondition(x => x.Id == id).FirstOrDefault();
            if (user is null)
            {
                throw new UserDoesNotExistException($"User with id {id} does not exist");
            }

            try
            {
                this.tokenGeneratorService.CheckToken(user, activationPayload.Token);
            }
            catch (InvalidTokenException ex)
            {
                if (ex.Message == "Token is outdated")
                {
                    this.userService.Delete(id);
                }

                throw new InvalidTokenException("Token is outdated. You have to register again");
            }
            catch (Exception)
            {
                throw;
            }

            this.userService.ActivateUser(id);
        }
    }
}
