using AutoMapper;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
using Core.Models.UserModels;
using Core.Settings;
using Microsoft.Extensions.Options;

namespace BLL.Services.UserServices
{
    internal class EditUserInfoService : IEditUserInfoService
    {
        private readonly IUserService userService;

        private readonly IEmailService emailService;

        private readonly IUserValidationService validationService;

        private readonly ITokenGeneratorService tokenGeneratorService;

        private readonly AppSettings appSettings;

        public EditUserInfoService(IUserService userService, IUserValidationService validationService, IEmailService emailService, IOptions<AppSettings> appSettings, ITokenGeneratorService tokenGeneratorService)
        {
            this.userService = userService;
            this.emailService = emailService;
            this.validationService = validationService;
            this.tokenGeneratorService = tokenGeneratorService;
            this.appSettings = appSettings.Value;
        }

        public async Task<ExceptionalResult> EditUser(UserEditModel editModel)
        {
            var validationResult = this.validationService.ValidateUpdateModel(editModel);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var updateModel = this.MapEditModelToUpdateModel(editModel);
            if (!string.IsNullOrWhiteSpace(editModel.Email))
            {
                updateModel.IsActive = false;
            }

            var updateResult = await this.userService.Update(updateModel);
            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            if (updateModel.Email is not null)
            {
                var user = updateResult.Value;
                await this.SendChangeEmail(user);
            }

            return new ExceptionalResult();
        }

        public bool CheckOldPassword(UserModel user, string oldPassword)
        {
            return this.userService.HashingService.Hash(oldPassword) == user.HashedPassword;
        }

        private async Task SendChangeEmail(UserModel user)
        {
            var uidb64 = this.tokenGeneratorService.GetUidb64(user);
            var token = this.tokenGeneratorService.GetToken(user);
            var subject = "Change email in your account";
            var url = $"{this.appSettings.Domain}/{uidb64}/{token}";
            var body = File.ReadAllText(this.appSettings.AccountActivationEmailTemplate)
                .Replace("{UserName}", user.UserName)
                .Replace("{url}", url);
            await this.emailService.SendEmail(user.Email, subject, body);
        }

        private UserUpdateModel MapEditModelToUpdateModel(UserEditModel editModel)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserEditModel, UserUpdateModel>().ForAllMembers(opt => opt.AllowNull());
            });
            var mapper = new Mapper(mapperConfiguration);

            var user = mapper.Map<UserUpdateModel>(editModel);
            user.IsActive = true;

            return user;
        }
    }
}
