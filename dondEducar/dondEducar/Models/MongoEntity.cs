
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dondEducar.Models
{
    public class MongoEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}