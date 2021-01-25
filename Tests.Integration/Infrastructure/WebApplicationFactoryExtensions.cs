using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration.Infrastructure
{
    internal static class WebApplicationFactoryExtensions
    {
        /* In normal circumstances I always find this kind of method useful to replace out-of-process dependencies
         * in individual tests. Here it is unused but I include it as an example. */
        internal static WebApplicationFactory<TStartup> WithServices<TStartup>(
            this WebApplicationFactory<TStartup> factory,
            Action<IServiceCollection> configureServices)
            where TStartup : class
        {
            return factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(configureServices));
        }
    }
}