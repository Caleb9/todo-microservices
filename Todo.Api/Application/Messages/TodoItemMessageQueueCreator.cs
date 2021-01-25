using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Todo.Api.Application.Messages
{
    /// <summary>
    ///     Attempts to ensure that "items" queue is created in RabbitMQ. Periodically retries to create the queue
    ///     until it succeeds.
    ///     See also <see cref="MessageBrokerServicesSetup"/>.
    /// </summary>
    internal class TodoItemMessageQueueCreator
        : BackgroundService
    {
        private readonly IConnectionFactory _factory;
        private readonly RetryIntervalInSeconds _retryInterval;

        public TodoItemMessageQueueCreator(
            IConnectionFactory factory,
            RetryIntervalInSeconds retryInterval)
        {
            _factory = factory;
            _retryInterval = retryInterval;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    using var connection = _factory.CreateConnection();
                    connection
                        .CreateModel()
                        .QueueDeclare(
                            "items" /* Hardcoded but should be a setting */,
                            true,
                            false,
                            false);
                    break;
                }
                catch (BrokerUnreachableException)
                {
                    await Task.Delay(
                        _retryInterval,
                        stoppingToken);
                }
            }
        }
    }
}