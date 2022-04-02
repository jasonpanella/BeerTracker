using System;
using System.Collections.Generic;
using System.Text;
using MongoDb.Repository.Interfaces;
using MongoDBLoader;

namespace MongoDb.Repository
{
    public class BeverageRepository:BaseRepository<Beverage>, IBeverageRepository
    {
        public BeverageRepository(IMongoDBContext context) : base(context)
        {

        }
    }
}
