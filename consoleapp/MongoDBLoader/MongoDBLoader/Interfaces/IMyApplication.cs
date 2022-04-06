using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDBLoader.Domain.CSVModel;

namespace MongoDBLoader
{
    public interface IMyApplication
    {
        Task<Beverage> FindOneDocumentAsync(string documentId);
        Task<Beverage> GetDetails(string documentId);
        Task<List<InputDataCsv>> ReadInputFile();

        Task InsertIntoMongoDb(List<InputDataCsv> inputDataList);
        Task<IEnumerable<Beverage>> GetAll();
        Task UpdateDocument(Beverage document);
        Task DeleteAllDocuments(FilterDefinition<Beverage> filterExpression);
        Task<long> GetDocumentCollectionCount(FilterDefinition<Beverage> filterExpression);
    }
}
