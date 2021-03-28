using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Dtos;
using Catalog.Models;
using Catalog.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
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
        public IEnumerable<ItemDto> GetItems()
        {
            // Get items from repository then copy each item to the DTO object.
            var items = _repository.GetItems()
                .Select(item => item.AsDto());

            return items;
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetItem(Guid id)
        {
            var item = _repository.GetItem(id);
            if (item is null)
            {
                return NotFound();
            }

            return Ok(item.AsDto());
        }

        // POST /items
        [HttpPost]
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            _repository.CreateItem(item);

            // For create operations, the convention is to return information about the newly created item.
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item.AsDto());
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto newItem)
        {
            var oldItem = _repository.GetItem(id);

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

            _repository.UpdateItem(item);

            // For update operations, the convention is to return no content.
            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public ActionResult RemoveItem(Guid id)
        {
            var item = _repository.GetItem(id);

            if(item is null)
            {
                return NotFound();
            }
            
            _repository.RemoveItem(id);
            
            return NoContent();
        }
    }
}