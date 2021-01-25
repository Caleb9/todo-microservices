using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Rebus.Bus;
using Todo.Api.Application.Messages;
using Todo.Shared;

namespace Todo.Api.Application.Services
{
    public sealed class MessagingTodoItemsServiceDecorator :
        ITodoItemsService
    {
        private readonly ITodoItemsService _decorated;
        private readonly IBus _bus;
        
        
        public MessagingTodoItemsServiceDecorator(
            ITodoItemsService decorated,
            IBus bus)
        {
            _decorated = decorated;
            _bus = bus;
        }
        
        public async Task<Result<Guid>> AddNewItemToList(
            Guid listId,
            string value)
        {
            var result = await _decorated.AddNewItemToList(listId, value);
            if (result.IsSuccess)
            {
                await _bus.Send(new NewItemMessage(listId, result.Value));
            }
            return result;
        }
    }
}