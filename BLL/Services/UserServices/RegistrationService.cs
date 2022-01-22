using AutoMapper;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;
using Core.Settings;
using Microsoft.Extensions.Options;

namespace BLL.Services.UserServices
{
    internal class RegistrationService : IRegistrationService
    {
        private readonly IUserValidationService validationService;

        private readonly IUserService userService;

        private readonly IEmailService emailService;

        private readonly AppSettings appSettings;

        private readonly ITokenGeneratorService tokenGeneratorService;

        public RegistrationService(
            IUserValidationService validationService,
            IUserService userService,
            IEmailService emailService,
            IOptions<AppSettings> appSettings,
            ITokenGeneratorService tokenGeneratorService)
        {
            this.validationService = validationService;
            this.userService = userService;
            this.emailService = emailService;
            this.appSettings = appSettings.Value;
            this.tokenGeneratorService = tokenGeneratorService;
        }

        public ExceptionalResult Register(UserRegistrationModel userData)
        {
            var result = this.validationService.Validate(userData);
            if (!result.IsSuccess)
            {
                return result;
            }

            var userModel = this.MapUserRegistrationModel(userData);
            var userResult = this.userService.CreateNonActiveUser(userModel);
            if (!userResult.IsSuccess)
            {
                return userResult;
            }

            this.SendActivationEmail(userResult.Value);
            return new ExceptionalResult();
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
