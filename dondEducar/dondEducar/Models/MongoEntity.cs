
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dondEducar.Models
{
    public class MongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}