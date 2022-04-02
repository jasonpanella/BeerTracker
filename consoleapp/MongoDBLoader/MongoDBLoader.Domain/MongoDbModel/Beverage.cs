using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBLoader
{
    public class Beverage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string BeverageName { get; set; }
        public string Description { get; set; }
        public decimal ABV { get; set; }
        public int Category { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}
