using System;
using System.Collections.Generic;
using Catalog.Models;

namespace Catalog.Repository
{
    public interface IItemsRepository
    {
        IEnumerable<Item> GetItems();
        Item GetItem(Guid id);
        void CreateItem(Item item);
        void UpdateItem(Item newItem);
        void RemoveItem(Guid id);
    }
}