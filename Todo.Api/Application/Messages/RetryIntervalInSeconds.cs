using System;

namespace Todo.Api.Application.Messages
{
    public sealed record RetryIntervalInSeconds(int Value)
    {
        public const string ConfigurationKey = "MessageBrokerRetryIntervalInSeconds";
        
        public static implicit operator TimeSpan(RetryIntervalInSeconds instance)
        {
            return TimeSpan.FromSeconds(instance.Value);
        }
    }
}