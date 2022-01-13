using AutoMapper;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.Models.UserModels;
using Core.Settings;
using Microsoft.Extensions.Options;

namespace BLL.Services.UserServices
{
    internal class RegistrationService : IRegistrationService
    {
        private readonly IValidationService validationService;

        private readonly IUserService userService;

        private readonly IEmailService emailService;

        private readonly AppSettings appSettings;

        private readonly ITokenGeneratorService tokenGeneratorService;

        public RegistrationService(
            IValidationService validationService,
            IUserService userService,
            IEmailService emailService,
            IOptions<AppSettings> appSettings,
            ITokenGeneratorService tokenGeneratorService)
        {
            this.validationService = validationService;
            this.userService = userService;
            this.emailService = emailService;
            this.appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            this.tokenGeneratorService = tokenGeneratorService;
        }

        public void Register(UserRegistrationModel userData)
        {
            this.validationService.Validate(userData);
            var userModel = this.MapUserRegistrationModel(userData);
            var user = this.userService.CreateNonActiveUser(userModel);

            this.SendActivationEmail(user);
        }

        private UserCreateModel MapUserRegistrationModel(UserRegistrationModel user)
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<UserRegistrationModel, UserCreateModel>());
            var mapper = new Mapper(mapperConfig);
            var userObject = mapper.Map<UserCreateModel>(user);

            return userObject;
        }

        private void SendActivationEmail(UserModel user)
        {
            if (user is null)
            {
                return;
            }

            var uidb64 = this.tokenGeneratorService.GetUidb64(user);
            var token = this.tokenGeneratorService.GetToken(user);
            var subject = "Email confirmation";
            var url = $"{this.appSettings.Domain}/{uidb64}/{token}";
            var body = File.ReadAllText(this.appSettings.AccountActivationEmailTemplate)
                           .Replace("{UserName}", user.UserName)
                           .Replace("{url}", url);
            this.emailService.SendEmail(user.Email, subject, body);
        }
    }
}
