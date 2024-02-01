using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace sdiagffa.test.host.utilities
{
    public class TestApplicationFactory : WebApplicationFactory<Program>
    {
        readonly Action<IServiceCollection> _configureServices;

        public TestApplicationFactory(Action<IServiceCollection> configureServices)
        {
            _configureServices = configureServices;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(_configureServices);
            return base.CreateHost(builder);
        }
    }
}
