using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using BLL.Services.RoomServices;
using BLL.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class DependencyRegistrar
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            ConfigureUserServices(services);
            ConfigureRoomServices(services);

            DAL.DependencyRegistrar.ConfigureServices(services);
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
        }

        private static void ConfigureRoomServices(IServiceCollection services)
        {
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IUserRoomService, UserRoomService>();
            services.AddScoped<IRoomValidationService, RoomValidationService>();
        }
    }
}
