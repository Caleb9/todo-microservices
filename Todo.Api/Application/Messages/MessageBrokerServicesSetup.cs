using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rebus.Injection;
using Rebus.ServiceProvider;

namespace Todo.Api.Application.Messages
{
    /// <summary>
    ///     Periodically tries to add Rebus to dependency container so that IBus can be used to send messages.
    ///     The service runs in background and if Rebus can establish a connection to RabbitMQ, then it finishes its
    ///     work.
    ///     This is of course a naive version, because it doesn't help if we loose connection later on. It does however
    ///     solve the problem of making RabbitMQ operational when running in our docker-compose example setup.
    ///     See also <see cref="TodoItemMessageQueueCreator"/>.
    /// </summary>
    public sealed class MessageBrokerServicesSetup :
        BackgroundService
    {
        private readonly IServiceProvider _diContainer;
        private readonly RetryIntervalInSeconds _retryInterval;

        public MessageBrokerServicesSetup(
            IServiceProvider diContainer,
            RetryIntervalInSeconds retryInterval)
        {
            _diContainer = diContainer;
            _retryInterval = retryInterval;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    _diContainer.UseRebus();
                    break;
                }
                catch (ResolutionException)
                {
                    await Task.Delay(
                        _retryInterval,
                        stoppingToken);
                }
            }
        }
    }
}