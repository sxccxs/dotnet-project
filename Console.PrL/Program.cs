﻿using BLL;
using Console.PrL.Interfaces;
using Console.PrL.Utilities;
using Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Console.PrL
{
    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<App>()?.StartApp();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            services.Configure<JsonDbSettings>(configuration.GetSection("JsonDb"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            DependencyRegistrar.ConfigureServices(services);
            services.AddScoped<IConsole, CustomConsole>();
            services.AddScoped<App>();
        }
    }
}