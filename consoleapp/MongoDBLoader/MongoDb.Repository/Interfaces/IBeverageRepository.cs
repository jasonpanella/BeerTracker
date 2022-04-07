using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDb.Repository.Interfaces;
using MongoDBLoader;

namespace MongoDb.Repository.Interfaces
{
    public interface IBeverageRepository: IBaseRepository<Beverage>
    {
       
    }
}
