using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Models;

namespace Catalog.Api.Repository
{
    public interface IItemsRepository
    {
        Task<IEnumerable<Item>> GetItemsAsync();
        Task<Item> GetItemAsync(Guid id);
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item newItem);
        Task RemoveItemAsync(Guid id);
    }
}