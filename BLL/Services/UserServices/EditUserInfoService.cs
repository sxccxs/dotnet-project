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

        public EditUserInfoService(IUserService userService, IAuthenticationService authenticationService, IEmailService emailService, AppSettings appSettings, ITokenGeneratorService tokenGeneratorService)
        {
            this.userService = userService;
            this.authenticationService = authenticationService;
            this.emailService = emailService;
            this.appSettings = appSettings;
            this.tokenGeneratorService = tokenGeneratorService;
        }

        public async Task<OptionalResult<string>> EditUser(OptionalResult<int> userId)
        {
            var user = (await this.userService.GetByCondition(x => x.Id == userId)).FirstOrDefault();
            if (user.Id.IsSuccess)
            {
            }
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
