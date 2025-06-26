using MongoDB.Driver;

namespace LIbrary.Command
{
    public class Operations<T>
    {
        private readonly IMongoDatabase _db = Commands.GetMongoDatabase();
        private readonly IMongoCollection<T> _collection;

        public Operations()
        {
            _collection = _db.GetCollection<T>(typeof(T).Name + "s");
        }

        public Operations(string collectionName)
        {
            _collection = _db.GetCollection<T>(collectionName);
        }
        public async Task AddOne(T doc)
        {
            await _collection.InsertOneAsync(doc);
        }

        public async Task<bool> Update(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            var updated = await _collection.UpdateOneAsync(filter, update);
            return updated.IsAcknowledged;
        }

        public async Task<bool> Delete(FilterDefinition<T> filter)
        {
            var deleted = await _collection.DeleteOneAsync(filter);
            return deleted.IsAcknowledged;

        }
        public async Task<T> Find(FilterDefinition<T> filter)
        {
            var doc = await _collection.Find(filter).FirstOrDefaultAsync();
            return doc;
        }

        public async Task<List<T>> FindMany(FilterDefinition<T> filter)
        {
            var docs = await _collection.Find(filter).ToListAsync();
            return docs;
        }

    }
}
