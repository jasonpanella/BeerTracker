using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDBLoader.Domain.CSVModel
{
    public class InputDataCsv
    {
        public string BeverageName { get; set;}
        public int Category { get; set; }
        public decimal ABV { get; set; }
        public string Description { get; set; }
    }
}
