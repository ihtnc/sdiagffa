using Flurl.Http;
using Flurl.Http.Configuration;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using sdiagffa.infrastructure.http;
using sdiagffa.infrastructure.models;
using System.Reflection;

namespace sdiagffa.infrastructure
{
    public static class StartupExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, ApiClientConfig config)
        {
            services
                .AddSingleton<IFlurlClientCache>(_ => new FlurlClientCache()
                    .Add(nameof(ISwapiClient), config.BaseUrl)

                )
                .AddTransient<ISwapiClient>(service =>
                {
                    var cache = service.GetService<IFlurlClientCache>();
                    var client = cache!
                        .Get(nameof(ISwapiClient))
                        .AllowAnyHttpStatus();
                    return new SwapiClient(client);
                });

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
