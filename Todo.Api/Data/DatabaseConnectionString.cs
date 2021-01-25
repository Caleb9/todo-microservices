using Todo.Shared;

namespace Todo.Api.Data
{
    public sealed record DatabaseConnectionString(string Value) :
        AbstractConnectionString(Value)
    {
        internal const string Name = "TodoApiDatabase";
    };
}