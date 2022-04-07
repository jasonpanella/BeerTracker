using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDb.Repository.Interfaces;
using MongoDBLoader;

namespace MongoDb.Repository
{
    public class BeverageRepository : BaseRepository<Beverage>, IBeverageRepository
    {

        public BeverageRepository()
            : this(new MongoDBContext())
        {
        }

        public BeverageRepository(MongoDBContext context) 
            : base(context)
        {
        }
    }
}
