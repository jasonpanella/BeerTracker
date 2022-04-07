using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDBLoader.Domain;

namespace MongoDBLoader
{
    [BsonCollection("Beverage")]
    public class Beverage : Document
    {
        public string BeverageName { get; set; }
        public string Description { get; set; }
        public decimal ABV { get; set; }
        public int Category { get; set; }
        public DateTime LastUpdate { get; set; }
        public string _partition { get; set; }
    }
}
