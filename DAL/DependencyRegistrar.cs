using DAL.Abstractions.Interfaces;
using DAL.ReaderWriters;
using DAL.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DependencyRegistrar
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppContext>(options => options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));
            services.AddScoped(typeof(IGenericStorageWorker<>), typeof(GenericDbWorker<>));
            services.AddScoped<IReaderWriter, JsonReaderWriter>();
        }
    }
}
