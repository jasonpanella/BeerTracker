using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDBLoader.Domain.CSVModel;
using Newtonsoft.Json;
using SharpCompress.Common;

namespace MongoDBLoader
{
    public class MyApplication: IMyApplication
    {
        private readonly MongoClient _client;
        protected IMongoCollection<Beverage> _dbCollection;
        private readonly Mongosettings _settings;
        private IMongoDatabase _db { get; set; }

        public MyApplication(MongoClient client, Mongosettings mongosettings)
        {
            _client = client;
            _settings = mongosettings;
        }

        public async Task GetDetails()
        {
            var _db = _client.GetDatabase(_settings.DatabaseName);
            _dbCollection = _db.GetCollection<Beverage>("Beverage");

            var objectId = new ObjectId("6244ad09c5008bff9191f1aa");
            FilterDefinition<Beverage> filter = Builders<Beverage>.Filter.Eq("_id", objectId);
            var document = await _dbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();
            Console.WriteLine(document.Description);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputDataList"></param>
        /// <returns></returns>
        public async Task InsertIntoMongoDb(List<InputDataCsv> inputDataList)
        {

            var _db = _client.GetDatabase(_settings.DatabaseName);
            _dbCollection = _db.GetCollection<Beverage>("Beverage");

            var count = await _dbCollection.CountDocumentsAsync(new BsonDocument());

            if (count > 0)
            {
                var filter = Builders<Beverage>.Filter.Empty;
                await _dbCollection.DeleteManyAsync(filter);
            }

            count = await _dbCollection.CountDocumentsAsync(new BsonDocument());

            List<Beverage> beverageList = new List<Beverage>();

            inputDataList.ForEach(x =>
            {
                beverageList.Add(new Beverage{Description = x.Description, ABV = x.ABV, Category = x.Category, BeverageName = x.BeverageName});

            });

            await _dbCollection.InsertManyAsync(beverageList);

            Console.WriteLine(await _dbCollection.CountDocumentsAsync(new BsonDocument()));

            
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
