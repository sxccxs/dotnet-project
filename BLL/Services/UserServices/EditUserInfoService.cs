using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;
using Core.Settings;

namespace BLL.Services.UserServices
{
    internal class EditUserInfoService : IEditUserInfoService
    {
        private readonly IUserService userService;

        private readonly IAuthenticationService authenticationService;

        private readonly IEmailService emailService;

        private readonly AppSettings appSettings;

        private readonly ITokenGeneratorService tokenGeneratorService;

        private readonly IUpdateValidationService updateValidationService;

        public EditUserInfoService(
            IUserService userService,
            IAuthenticationService authenticationService,
            IEmailService emailService,
            AppSettings appSettings,
            ITokenGeneratorService tokenGeneratorService,
            IUpdateValidationService updateValidationService)
        {
            this.userService = userService;
            this.authenticationService = authenticationService;
            this.emailService = emailService;
            this.appSettings = appSettings;
            this.tokenGeneratorService = tokenGeneratorService;
            this.updateValidationService = updateValidationService;
        }

        public async Task<OptionalResult<string>> EditUser(OptionalResult<UserUpdateModel> user, string updatedInfo)
        {
            switch (updatedInfo)
            {
                case "password": this.updateValidationService.ValidatePassword(user.Value.Password);
                    break;

                case "email": this.updateValidationService.ValidatePassword(user.Value.Email);
                    break;

                case "username": this.updateValidationService.ValidatePassword(user.Value.UserName);
                    break;
                default: return new OptionalResult<string>("wrong info");
            }

            var result = await this.userService.Update(user.Value);
            if (result.IsSuccess)
            {
                await this.SendChangesEmail(result.Value);
                return new OptionalResult<string>(true, "Successfully changed user information.");
            }

            return new OptionalResult<string>(false, "User Information was not changed.");
        }

        private async Task SendChangesEmail(UserModel user)
        {
            if (user is null)
            {
                return;
            }

            var uidb64 = this.tokenGeneratorService.GetUidb64(user);
            var token = this.tokenGeneratorService.GetToken(user);
            var subject = "Changes in your account information";
            var url = $"{this.appSettings.Domain}/{uidb64}/{token}";
            var body = File.ReadAllText(this.appSettings.AccountActivationEmailTemplate)
                .Replace("{UserName}", user.UserName)
                .Replace("{url}", url);
            await this.emailService.SendEmail(user.Email, subject, body);
        }
    }
}
