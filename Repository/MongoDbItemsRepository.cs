using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repository
{
    public class MongoDbItemsRepository : IItemsRepository
    {
        private const string dbName = "catalogBD";
        private const string collectionName = "items";
        
        private readonly IMongoCollection<Item> _itemsCollection;
        private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(dbName);
            _itemsCollection = database.GetCollection<Item>(collectionName);
        }
        public async Task CreateItemAsync(Item item)
        {
            await _itemsCollection.InsertOneAsync(item);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            return await _itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await _itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task RemoveItemAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            await _itemsCollection.DeleteOneAsync(filter);
        }

        public async Task UpdateItemAsync(Item newItem)
        {
            var filter = _filterBuilder.Eq(oldItem => oldItem.Id, newItem.Id);
            await _itemsCollection.ReplaceOneAsync(filter, newItem);
        }
    }
}