using BLL.Abstractions.Interfaces;
using BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class DependencyRegistrar
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IHashingService, SHA256HashingService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IAccountActivationService, AccountActivationService>();
            services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();

            DAL.DependencyRegistrar.ConfigureServices(services);
        }
    }
}
