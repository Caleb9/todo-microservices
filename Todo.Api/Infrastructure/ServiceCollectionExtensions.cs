using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Injection;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Todo.Api.Application.Messages;
using Todo.Api.Application.Services;
using Todo.Api.Data;
using Todo.Shared;

namespace Todo.Api.Infrastructure
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDataLayerDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                .AddSingleton(
                    new DatabaseConnectionString(
                        configuration.GetConnectionString(DatabaseConnectionString.Name)))
                .AddHostedService<DatabaseMigrator>()
                .AddDbContext<TodoContext>();
        }

        internal static IServiceCollection AddMessageBrokerDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                .AddSingleton(
                    new MessageBrokerConnectionString(
                        configuration.GetConnectionString(MessageBrokerConnectionString.Name)))
                .AddRebus((config, diContainer) =>
                    config
                        .Transport(t =>
                            /* Would be nice to find a way to replace this in integration tests with in-memory queue */
                            t.UseRabbitMqAsOneWayClient(
                                diContainer.GetRequiredService<MessageBrokerConnectionString>()))
                        .Routing(r =>
                            r.TypeBased().Map<NewItemMessage>("items" /* Hardcoded but should be a setting */)));
        }

        internal static IServiceCollection AddMessageBrokerBackgroundServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                .AddSingleton(
                    new RetryIntervalInSeconds(
                        configuration.GetValue<int>(RetryIntervalInSeconds.ConfigurationKey)))
                .AddSingleton<IConnectionFactory>(diContainer =>
                    new ConnectionFactory
                    {
                        Uri = new Uri(diContainer.GetRequiredService<MessageBrokerConnectionString>())
                    })
                .AddHostedService<TodoItemMessageQueueCreator>()
                .AddHostedService<MessageBrokerServicesSetup>();
        }

        internal static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            return services
                .AddSingleton<NoOpBus>()
                .AddScoped<TodoItemsService>()
                .AddScoped<ITodoItemsService>(s =>
                {
                    /* Factory for ITodoItemsService.
                     * If Rebus can establish connection to RabbitMQ queue then a "real" IBus is used. Otherwise NoOpBus
                     * which doesn't do anything.
                     * Feels hackish, and there's probably a better way but it would require getting more intimate
                     * knowledge of Rebus. */
                    IBus bus;
                    try
                    {
                        bus = s.GetRequiredService<IBus>();
                    }
                    catch (ResolutionException)
                    {
                        bus = s.GetRequiredService<NoOpBus>();
                    }
                    /* Decorate TodoItemsService with MessagingTodoItemsServiceDecorator. */
                    return new MessagingTodoItemsServiceDecorator(
                        s.GetRequiredService<TodoItemsService>(),
                        bus);
                });
        }
    }
}