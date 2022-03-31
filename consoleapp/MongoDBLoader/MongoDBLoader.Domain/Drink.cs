using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBLoader
{
    public class Drink
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal IsComplete { get; set; }

        public string Summary { get; set; }

        public decimal AbvContext { get; set; }
    }
}
