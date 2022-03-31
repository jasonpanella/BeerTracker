using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SharpCompress.Common;

namespace MongoDBLoader
{
    public class MyApplication: IMyApplication
    {
        private readonly MongoClient _client;
        protected IMongoCollection<Drink> _dbCollection;
        private readonly Mongosettings _settings;
        private IMongoDatabase _db { get; set; }

        public MyApplication(MongoClient client, Mongosettings mongosettings)
        {
            _client = client;
            _settings = mongosettings;
        }

        public async Task GetDetails()
        {
            //var objectId = new ObjectId("6244ad09c5008bff9191f1aa");
            //FilterDefinition<Drink> filter = Builders<Drink>.Filter.Eq("_id", objectId);
            //var result = await _dbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();
            //Console.WriteLine(result);

            var dbList = _client.ListDatabases().ToList();

            Console.WriteLine("The list of databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }



        }


    }
}
