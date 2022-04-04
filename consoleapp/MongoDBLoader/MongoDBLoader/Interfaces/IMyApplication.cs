using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDBLoader.Domain.CSVModel;

namespace MongoDBLoader
{
    public interface IMyApplication
    {
        Task GetDetails();
        Task<List<InputDataCsv>> ReadInputFile();

        Task InsertIntoMongoDb(List<InputDataCsv> inputDataList);
        Task GetAll();
    }
}
