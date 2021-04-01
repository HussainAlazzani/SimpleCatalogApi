using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Dtos;
using Catalog.Api.Models;
using Catalog.Api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;

        public ItemsController(IItemsRepository repository)
        {
            _repository = repository;
        }

        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            // Get items from repository then copy each item to the DTO object.
            var items = (await _repository.GetItemsAsync())
                .Select(item => item.AsDto());

            return items;
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await _repository.GetItemAsync(id);
            if (item is null)
            {
                return NotFound();
            }

            return Ok(item.AsDto());
        }

        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _repository.CreateItemAsync(item);

            // For create operations, the convention is to return information about the newly created item.
            // Note, .NET overrides the async suffix from nameof(GetItemAsync). 
            // You must disable this feature in Startup by making that option false inside AddControllers().
            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto newItem)
        {
            var oldItem = await _repository.GetItemAsync(id);

            if (oldItem is null)
            {
                return NotFound();
            }

            // Copying to the existing item. Old Id and CreatedDate won't change.
            var item = oldItem with
            {
                Name = newItem.Name,
                Price = newItem.Price,
            };

            await _repository.UpdateItemAsync(item);

            // For update operations, the convention is to return no content.
            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveItemAsync(Guid id)
        {
            var item = await _repository.GetItemAsync(id);

            if(item is null)
            {
                return NotFound();
            }
            
            await _repository.RemoveItemAsync(id);
            
            return NoContent();
        }
    }
}