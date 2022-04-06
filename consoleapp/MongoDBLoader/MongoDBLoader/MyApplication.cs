using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CsvHelper;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDb.Repository.Interfaces;
using MongoDBLoader.Domain;
using MongoDBLoader.Domain.CSVModel;

namespace MongoDBLoader
{
    public class MyApplication: IMyApplication
    {
        private readonly MongoClient _client;
        protected IMongoCollection<Beverage> _dbCollection;
        private readonly Mongosettings _settings;
        private IMongoDatabase _db { get; set; }
        private readonly IBeverageRepository _beverageRepository;


        public MyApplication(MongoClient client, Mongosettings mongosettings, IBeverageRepository beverageRepository)
        {
            _client = client;
            _settings = mongosettings;
            _beverageRepository = beverageRepository;
        }

        /// <summary>
        /// Get all Beverage documents
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Beverage>> GetAll()
        {
            return _beverageRepository.AsQueryable();
        }
        
        public async Task<Beverage> GetDetails(string documentId)
        {
            return await _beverageRepository.FindByIdAsync(documentId);
        }

        public async Task UpdateDocument(Beverage document)
        {
            await _beverageRepository.Update(document);
        }

        public async Task DeleteAllDocuments(FilterDefinition<Beverage> filterExpression)
        {
            await _beverageRepository.DeleteAllAsync(filterExpression);
        }

        public async Task<long> GetDocumentCollectionCount(FilterDefinition<Beverage> filterExpression)
        {
            return await _beverageRepository.DocumentCount(filterExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputDataList"></param>
        /// <returns></returns>
        public async Task InsertIntoMongoDb(List<InputDataCsv> inputDataList)
        {

            //var _db = _client.GetDatabase(_settings.DatabaseName);
            //_dbCollection = _db.GetCollection<Beverage>("Beverage");

            var count = await _beverageRepository.DocumentCount(Builders<Beverage>.Filter.Empty);

            if (count > 0)
            {
                await DeleteAllDocuments(Builders<Beverage>.Filter.Empty);
            }

            count = await _beverageRepository.DocumentCount(Builders<Beverage>.Filter.Empty);
            //_dbCollection.CountDocumentsAsync(new BsonDocument());

            List<Beverage> beverageList = new List<Beverage>();

            inputDataList.ForEach(x =>
            {
                beverageList.Add(new Beverage{Description = x.Description, ABV = x.ABV, Category = x.Category, BeverageName = x.BeverageName});

            });

            await _beverageRepository.InsertManyAsync(beverageList);

            await _dbCollection.InsertManyAsync(beverageList);

            Console.WriteLine(await _dbCollection.CountDocumentsAsync(new BsonDocument()));

            
        }

        public async Task<Beverage> FindOneDocumentAsync(string documentId)
        {
            var objectId = new ObjectId(documentId);
            return await _beverageRepository.FindOneAsync(doc=> doc.Id == objectId);
        }


        /// <summary>
        /// Reads the input data file and loads into input csv object.
        /// </summary>
        /// <returns>List<InputDataCsv> A list of input data csvs records</returns>
        public async Task<List<InputDataCsv>> ReadInputFile()
        {
            List<InputDataCsv> inputDataCsvList = new List<InputDataCsv>();

            //load file
            using (var reader = new StreamReader($"{System.AppDomain.CurrentDomain.BaseDirectory}/Utilities/beers.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                inputDataCsvList = csv.GetRecords<InputDataCsv>().ToList();
            }

            return inputDataCsvList;
        }

    }
}
