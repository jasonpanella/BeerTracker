using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDBLoader.Domain;

namespace MongoDb.Repository.Interfaces
{
    public interface IBaseRepository<TDocument> where TDocument : IDocument
    {
        IQueryable<TDocument> AsQueryable();
        IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filterExpression);
        Task<TDocument> FindByIdAsync(string id);
        Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);
        Task InsertManyAsync(ICollection<TDocument> documents);
        Task InsertOneAsync(TDocument document);
        Task Update(TDocument document);
        Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);
        Task DeleteByIdAsync(string id);
        void DeleteById(string id);
        Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);
        Task<TDocument> Get(string id);
        Task<IEnumerable<TDocument>> Get();
        Task<long> DocumentCount(FilterDefinition<TDocument> filterExpression);
        Task DeleteAllAsync(FilterDefinition<TDocument> filterExpression);
    }
}
