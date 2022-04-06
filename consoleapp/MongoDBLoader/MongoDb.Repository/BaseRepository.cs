using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDb.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDBLoader.Domain;

namespace MongoDb.Repository
{
    public abstract class BaseRepository<TDocument> : IBaseRepository<TDocument> where TDocument : IDocument
    {
        protected readonly IMongoDBContext _mongoContext;
        protected IMongoCollection<TDocument> _dbCollection;

        protected BaseRepository(IMongoDBContext context)
        {
            _mongoContext = context;
            _dbCollection = _mongoContext.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        private protected string GetCollectionName(Type documentType)
        {
            var foo = ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;

            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }

        public virtual IQueryable<TDocument> AsQueryable()
        {
            return  _dbCollection.AsQueryable();
        }

        public virtual IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filterExpression)
        {
            return _dbCollection.Find(filterExpression).ToEnumerable();
        }

        public virtual Task<TDocument> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
                return _dbCollection.Find(filter).SingleOrDefaultAsync();
            });
        }

        public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _dbCollection.Find(filterExpression).FirstOrDefaultAsync());
        }


        public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            //ex. 5dc1039a1521eaa36835e541
            return Task.Run(() => _dbCollection.FindOneAndDeleteAsync(filterExpression));
        }

        public void DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            _dbCollection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
                _dbCollection.FindOneAndDeleteAsync(filter);
            });
        }

        public async Task DeleteAllAsync(FilterDefinition<TDocument> filterExpression)
        {
            await _dbCollection.DeleteManyAsync(filterExpression);
        }

        public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _dbCollection.DeleteManyAsync(filterExpression));
        }


        public virtual Task InsertOneAsync(TDocument document)
        {
            return Task.Run(() => _dbCollection.InsertOneAsync(document));
        }

        public async Task<TDocument> Get(string id)
        {
            //ex. 5dc1039a1521eaa36835e541

            var objectId = new ObjectId(id);
           
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            return await _dbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();

        }

        public async Task<long> DocumentCount(FilterDefinition<TDocument> filterExpression)
        {
            return await _dbCollection.CountDocumentsAsync(filterExpression);
        }

        public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
        {
            await _dbCollection.InsertManyAsync(documents);
        }


        public async Task<IEnumerable<TDocument>> Get()
        {
            return _dbCollection.AsQueryable();
        }

        public virtual async Task Update(TDocument obj)
        {
           await _dbCollection.ReplaceOneAsync(Builders<TDocument>.Filter.Eq("_id", obj.Id), obj);
        }
    }
}
