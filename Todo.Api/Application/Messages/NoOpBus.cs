using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Bus.Advanced;

namespace Todo.Api.Application.Messages
{
    /// <summary>
    ///     Used if RabbitMQ connection cannot be established.
    /// </summary>
    internal sealed class NoOpBus
        : IBus
    {
        private readonly ILogger<NoOpBus> _logger;

        public NoOpBus(
            ILogger<NoOpBus> logger)
        {
            _logger = logger;
        }

        void IDisposable.Dispose()
        {
        }

        Task IBus.SendLocal(
            object commandMessage, IDictionary<string, string> optionalHeaders)
        {
            return LogWarning();
        }

        Task IBus.Send(
            object commandMessage, IDictionary<string, string> optionalHeaders)
        {
            return LogWarning();
        }

        Task IBus.DeferLocal(
            TimeSpan delay, object message, IDictionary<string, string> optionalHeaders)
        {
            return LogWarning();
        }

        Task IBus.Defer(
            TimeSpan delay, object message, IDictionary<string, string> optionalHeaders)
        {
            return LogWarning();
        }

        Task IBus.Reply(
            object replyMessage, IDictionary<string, string> optionalHeaders)
        {
            return LogWarning();
        }

        Task IBus.Subscribe<TEvent>()
        {
            return LogWarning();
        }

        Task IBus.Subscribe(
            Type eventType)
        {
            return LogWarning();
        }

        Task IBus.Unsubscribe<TEvent>()
        {
            return LogWarning();
        }

        Task IBus.Unsubscribe(
            Type eventType)
        {
            return LogWarning();
        }

        Task IBus.Publish(
            object eventMessage, IDictionary<string, string> optionalHeaders)
        {
            return LogWarning();
        }

        IAdvancedApi IBus.Advanced { get; } = null!;

        private Task LogWarning()
        {
            _logger.LogWarning("Message Broker connection is currently not available.");
            return Task.CompletedTask;
        }
    }
}