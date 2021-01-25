using System.IO;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Rebus.Bus;
using Rebus.Transport;
using Todo.Api;

namespace Tests.Integration.Infrastructure
{
    [UsedImplicitly]
    public sealed class TodoApiApplicationFactory :
        WebApplicationFactory<Startup>
    {
        internal const string TestingDatabaseFile = nameof(ApiTests) + ".sqlite";

        protected override IHost CreateHost(
            IHostBuilder builder)
        {
            CleanUpTestingDatabase();
            ReplaceDatabaseConnectionString(builder);
            ReplaceRebus(builder);
            return base.CreateHost(builder);
        }

        private static void CleanUpTestingDatabase()
        {
            File.Delete(TestingDatabaseFile);
        }

        private static void ReplaceDatabaseConnectionString(
            IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
                services.Replace(
                    ServiceDescriptor.Singleton(
                        TestingDatabaseConnectionString.Instance)));
        }

        private void ReplaceRebus(
            IHostBuilder builder)
        {
            /* We should not attempt to create "real" RabbitMQ connections in integration tests.
             * This is not a complete replacement to Rebus infrastructure. I need to learn more about how to test an app
             * using Rebus. Currently it seems that despite using interfaces all over this library it is difficult to
             * re-configure in tests (calling services.AddRebus more than once throws an exception). I would need to
             * make more infrastructure to prevent calling it on Startup in tests in the first place. I leave this as
             * TODO. */
            builder.ConfigureServices(services =>
                services
                    .RemoveAll<ITransport>()
                    .RemoveAll<IBus>()
                    .AddSingleton(Mock.Of<IBus>()));
        }
    }
}