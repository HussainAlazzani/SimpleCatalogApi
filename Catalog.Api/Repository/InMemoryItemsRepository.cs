using System;
using System.Collections.Generic;
using Catalog.Api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Repository
{
    public class InMemoryItemsRepository : IItemsRepository
    {
        private readonly List<Item> items = new()
        {
            new Item
            {
                Id = Guid.NewGuid(),
                Name = "Potion",
                Price = 9,
                CreatedDate = DateTimeOffset.UtcNow
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Name = "Iron Swod",
                Price = 20,
                CreatedDate = DateTimeOffset.UtcNow
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Name = "Bronze Shield",
                Price = 18,
                CreatedDate = DateTimeOffset.UtcNow
            }
        };

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            // Since we don't have anything to call, we simply wrap our
            // collection inside a task.
            return await Task.FromResult(items);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var item = items.Where(item => item.Id == id).SingleOrDefault();
            return await Task.FromResult(item);
        }

        public async Task CreateItemAsync(Item item)
        {
            items.Add(item);

            // We have nothing to return so we simply trigger a completed task.
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item newItem)
        {
            var index = items.FindIndex(oldItem => oldItem.Id == newItem.Id);
            items[index] = newItem;

            // We have nothing to return so we simply trigger a completed task.
            await Task.CompletedTask;
        }

        public async Task RemoveItemAsync(Guid id)
        {
            var index = items.FindIndex(item => item.Id == id);
            items.RemoveAt(index);

            // We have nothing to return so we simply trigger a completed task.
            await Task.CompletedTask;
        }
    }
}