using System;
using System.Collections.Generic;
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
        public void CreateItem(Item item)
        {
            _itemsCollection.InsertOne(item);
        }

        public Item GetItem(Guid id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            return _itemsCollection.Find(filter).SingleOrDefault();
        }

        public IEnumerable<Item> GetItems()
        {
            return _itemsCollection.Find(new BsonDocument()).ToList();
        }

        public void RemoveItem(Guid id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            _itemsCollection.DeleteOne(filter);
        }

        public void UpdateItem(Item newItem)
        {
            var filter = _filterBuilder.Eq(oldItem => oldItem.Id, newItem.Id);
            _itemsCollection.ReplaceOne(filter, newItem);
        }
    }
}