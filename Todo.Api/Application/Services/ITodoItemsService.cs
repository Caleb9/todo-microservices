using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Todo.Api.Application.Services
{
    public interface ITodoItemsService
    {
        Task<Result<Guid>> AddNewItemToList(
            Guid listId,
            string value);
    }
}