using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace MongoDBLoader
{

    public interface IMongoDBContext
    {
        IMongoCollection<Book> GetCollection<Book>(string name);
    }

}
