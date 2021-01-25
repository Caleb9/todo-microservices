using System.Threading;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Hosting;

namespace Todo.Api.Data
{
    internal sealed class DatabaseMigrator :
        IHostedService
    {
        private readonly DatabaseConnectionString _connectionString;

        public DatabaseMigrator(
            DatabaseConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            var migrator =
                DeployChanges.To.SQLiteDatabase(_connectionString)
                    .WithScriptsEmbeddedInAssembly(typeof(DatabaseMigrator).Assembly)
                    .LogToConsole()
                    .Build();

            var result = migrator.PerformUpgrade();
            if (result.Successful is false)
            {
                throw result.Error;
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}