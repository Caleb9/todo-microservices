using Todo.Shared;

namespace Todo.Api.Application.Messages
{
    public sealed record MessageBrokerConnectionString(string Value) :
        AbstractConnectionString(Value)
    {
        internal const string Name = "MessageBroker";
    }
}