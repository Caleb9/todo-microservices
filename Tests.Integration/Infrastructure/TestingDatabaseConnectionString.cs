using Todo.Api.Data;

namespace Tests.Integration.Infrastructure
{
    internal static class TestingDatabaseConnectionString
    {
        internal static readonly DatabaseConnectionString Instance =
            new ($"Data Source={TodoApiApplicationFactory.TestingDatabaseFile}");
    }
}