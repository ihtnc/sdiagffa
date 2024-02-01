using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using sdiagffa.test.host.utilities;
using System;

namespace sdiagffa.test.host
{
    public abstract class BaseControllerTests
    {
        protected static FlurlClient CreateClient(Action<IServiceCollection> configureServices)
        {
            var factory = new TestApplicationFactory(configureServices);
            var client = factory.CreateClient();
            return new FlurlClient(client);
        }
    }
}
