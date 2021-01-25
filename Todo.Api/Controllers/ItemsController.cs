using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Api.Application.Services;
using Todo.Api.Data;

namespace Todo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ITodoItemsService _service;
        private readonly TodoContext _context;

        public ItemsController(
            TodoContext context,
            ITodoItemsService service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Post(
            ItemsPostRequestDto dto)
        {
            var result = await _service.AddNewItemToList(dto.ListId, dto.Value);
            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }
            await _context.SaveChangesAsync();

            return result.Value;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemsGetResponseDto>> Get(
            Guid id)
        {
            var item = await _context.TodoItems.SingleOrDefaultAsync(i => i.Id == id);
            if (item is null)
            {
                return NotFound($"Item {id} not found");
            }

            return new ItemsGetResponseDto
            {
                Id = item.Id,
                Value = item.Value
            };
        }

        public class ItemsPostRequestDto
        {
            [Required]
            [UsedImplicitly(ImplicitUseKindFlags.Assign)]
            public Guid ListId { get; init; }

            [Required]
            [UsedImplicitly(ImplicitUseKindFlags.Assign)]
            public string Value { get; init; } = string.Empty;
        }

        public class ItemsGetResponseDto
        {
            [UsedImplicitly(ImplicitUseKindFlags.Access)]
            public Guid Id { get; init; }

            [UsedImplicitly(ImplicitUseKindFlags.Access)]
            public string Value { get; init; } = string.Empty;
        }
    }
}