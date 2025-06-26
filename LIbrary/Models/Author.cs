using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LIbrary.Models
{
    public class Author
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("Books")]
        public List<ObjectId>? Books { get; set; } = [];

    }
}
