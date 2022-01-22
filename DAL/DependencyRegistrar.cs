using DAL.Abstractions.Interfaces;
using DAL.ReaderWriters;
using DAL.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DependencyRegistrar
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericStorageWorker<>), typeof(GenericJsonWorker<>));
            services.AddScoped<IReaderWriter, JsonReaderWriter>();
        }
    }
}
