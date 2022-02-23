using Core.Settings;

namespace ASP.NET_PrL;

public static class DependencyRegistrar
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JsonDbSettings>(configuration.GetSection("JsonDb"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        BLL.DependencyRegistrar.ConfigureServices(services, configuration);
    }
}
