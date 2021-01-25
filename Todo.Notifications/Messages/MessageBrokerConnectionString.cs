using Todo.Shared;

namespace Todo.Notifications.Messages
{
    public sealed record MessageBrokerConnectionString(string Value) :
        AbstractConnectionString(Value)
    {
        internal const string Name = "MessageBroker";
    }
}