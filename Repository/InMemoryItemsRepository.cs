using System;
using System.Collections.Generic;
using Catalog.Models;
using System.Linq;

namespace Catalog.Repository
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

        public IEnumerable<Item> GetItems()
        {
            return items;
        }

        public Item GetItem(Guid id)
        {
            return items.Where(item => item.Id == id).SingleOrDefault();
        }

        public void CreateItem(Item item)
        {
            items.Add(item);
        }

        public void UpdateItem(Item newItem)
        {
            var index = items.FindIndex(oldItem => oldItem.Id == newItem.Id);
            items[index] = newItem;
        }

        public void RemoveItem(Guid id)
        {
            var index = items.FindIndex(item => item.Id == id);
            items.RemoveAt(index);
        }
    }
}