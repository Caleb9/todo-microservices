using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Tests.Integration.Infrastructure
{
    internal static class TestingDatabase
    {
        internal static async Task<int> Execute(string sql)
        {
            await using var connection = NewConnection();
            return await connection.ExecuteAsync(sql);
        }

        internal static async Task<IEnumerable<T>> Query<T>(string sql)
        {
            await using var connection = NewConnection();
            return await connection.QueryAsync<T>(sql);
        }

        private static SqliteConnection NewConnection()
        {
            return new(TestingDatabaseConnectionString.Instance);
        }
    }
}