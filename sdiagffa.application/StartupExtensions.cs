using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace sdiagffa.application
{
    public static class StartupExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining(typeof(StartupExtensions)));

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
