using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Tests.Integration.Infrastructure;
using Todo.Api.Controllers;
using Todo.Api.Domain;
using Xunit;

namespace Tests.Integration
{
    public sealed class ApiTests :
        IClassFixture<TodoApiApplicationFactory>
    {
        private readonly TodoApiApplicationFactory _factory;

        public ApiTests(
            TodoApiApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_to_lists_endpoint_adds_new_list_to_database()
        {
            using var client = _factory.CreateDefaultClient();

            using var response =
                await client.PostAsync(
                    "/lists",
                    JsonContent.Create(
                        new ListsController.ListPostRequestDto
                        {
                            Name = "test list"
                        }));

            using var _ = new AssertionScope();
            response.IsSuccessStatusCode.Should().BeTrue();
            var lists =
                await TestingDatabase.Query<TodoList>(
                    "SELECT * FROM TodoLists");
            lists.Should().Contain(tl =>
                tl.Name == "test list");
        }

        [Fact]
        public async Task Post_to_items_endpoint_adds_new_item_to_list()
        {
            var listId = Guid.NewGuid();
            using var client = _factory.CreateDefaultClient();
            await TestingDatabase.Execute(
                /* There seems to be a bug in SQLite data provider for EF Core where Guids saved in the
                 * database as lower-cased strings do not work in queries.
                 * See https://github.com/dotnet/efcore/issues/19651. */
                $"INSERT INTO TodoLists VALUES ('{listId.ToString().ToUpper()}', 'test list')");

            using var response =
                await client.PostAsync(
                    "/items",
                    JsonContent.Create(
                        new ItemsController.ItemsPostRequestDto
                        {
                            ListId = listId,
                            Value = "do stuff"
                        }));

            using var _ = new AssertionScope();
            response.IsSuccessStatusCode.Should().BeTrue();
            var items =
                await TestingDatabase.Query<TodoItem>(
                    "SELECT * FROM TodoItems");
            items.Should().Contain(ti =>
                ti.Value == "do stuff" &&
                ti.TodoListId == listId);
            /* TODO it would be worth to assert that IBus sends a message to message broker */
        }
    }
}