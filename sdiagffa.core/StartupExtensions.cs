using Microsoft.Extensions.DependencyInjection;
using sdiagffa.core.models;
using sdiagffa.core.services;
using sdiagffa.infrastructure;

namespace sdiagffa.core
{
    public static class StartupExtensions
    {
        public static void AddCore(this IServiceCollection services, CoreConfig config)
        {
            services.AddInfrastructure(config.ApiClient);

            services
                .AddTransient<IConnectionCrawler, ConnectionCrawler>()
                .AddSingleton<IRelationshipManager, RelationshipManager>()
                .AddSingleton<IStarwarsObjectProvider, StarwarsObjectProvider>();
        }
    }
}
