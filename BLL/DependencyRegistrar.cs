using BLL.Abstractions.Interfaces.RoleInterfaces;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using BLL.Services.RoleServices;
using BLL.Services.RoomServices;
using BLL.Services.UserServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class DependencyRegistrar
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureUserServices(services);
            ConfigureRoomServices(services);
            ConfigureRoleServices(services);

            DAL.DependencyRegistrar.ConfigureServices(services, configuration);
        }

        private static void ConfigureUserServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IHashingService, SHA256HashingService>();
            services.AddScoped<IUserValidationService, UserValidationService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IAccountActivationService, AccountActivationService>();
            services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEditUserInfoService, EditUserInfoService>();
        }

        private static void ConfigureRoomServices(IServiceCollection services)
        {
            services.AddScoped<IRoleTypeService, RoleTypeService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IUserRoomService, UserRoomService>();
            services.AddScoped<IRoomValidationService, RoomValidationService>();
        }

        private static void ConfigureRoleServices(IServiceCollection services)
        {
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoomRoleService, UserRoomRoleService>();
        }
    }
}
