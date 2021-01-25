using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.Api.Domain;

namespace Todo.Api.Data
{
    public sealed class TodoContext :
        DbContext
    {
        private readonly DatabaseConnectionString _connectionString;
        private readonly bool _isDevelopment;

        public TodoContext(
            DbContextOptions<TodoContext> options,
            DatabaseConnectionString connectionString,
            IWebHostEnvironment environment)
            : base(options)
        {
            _connectionString = connectionString;
            _isDevelopment = environment.IsDevelopment();
        }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        internal DbSet<TodoList> TodoLists { get; private init; } = null!;

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        internal DbSet<TodoItem> TodoItems { get; private init; } = null!;

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
            if (_isDevelopment)
            {
                /* Print SQL queries to console */
                optionsBuilder
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, (_, level) => level == LogLevel.Information);
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}