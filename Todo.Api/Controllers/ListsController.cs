using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Api.Data;
using Todo.Api.Domain;

namespace Todo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListsController : ControllerBase
    {
        private readonly TodoContext _context;

        public ListsController(
            TodoContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<Guid> Post(
            ListPostRequestDto dto)
        {
            var todoList = new TodoList(Guid.NewGuid(), dto.Name);
            await _context.TodoLists.AddAsync(todoList);
            await _context.SaveChangesAsync();
            return todoList.Id;
        }

        [HttpGet]
        public async Task<IEnumerable<Guid>> Get()
        {
            return await _context.TodoLists.Select(tl => tl.Id).ToListAsync();
        }

        /* With C# 9.0 the DTOs beg to be implemented as records. Unfortunately Swagger
         * does not seem to understand attribute annotations on records currently :( */
        
        public class ListPostRequestDto
        {
            [Required] public string Name { get; init; } = string.Empty;
        }
    }
}