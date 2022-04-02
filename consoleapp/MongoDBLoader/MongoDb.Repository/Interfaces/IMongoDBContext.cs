using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace MongoDb.Repository.Interfaces
{

    public interface IMongoDBContext
    {
        IMongoCollection<Beverage> GetCollection<Beverage>(string name);
    }

}
