using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace sdiagffa.test.host.utilities
{
    public static class ServiceCollectionExtensions
    {
        public static void SwapTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            var descriptors = services.Where(s => s.ServiceType == typeof(TService) && s.Lifetime == ServiceLifetime.Transient).ToArray();
            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddTransient(implementationFactory);
        }
    }
}
