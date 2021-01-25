using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Todo.Api.Data;
using Todo.Api.Domain;

namespace Todo.Api.Application.Services
{
    public sealed class TodoItemsService :
        ITodoItemsService
    {
        private readonly TodoContext _context;

        public TodoItemsService(
            TodoContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> AddNewItemToList(
            Guid listId,
            string value)
        {
            var listExists = await _context.TodoLists.AnyAsync(l => l.Id == listId);
            if (listExists is false)
            {
                return Result.Failure<Guid>($"List {listId} not found.");
            }

            var newItem = new TodoItem(Guid.NewGuid(), value, listId);
            await _context.TodoItems.AddAsync(newItem);

            return newItem.Id;
        }
    }
}