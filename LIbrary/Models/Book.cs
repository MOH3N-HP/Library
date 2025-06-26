using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace LIbrary.Models
{
    public class Book
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; } = null!;

        [BsonElement("Description")]
        public string Description { get; set; } = null!;

        [BsonElement("Authors")]
        public List<ObjectId>? Authors { get; set; } = [];

        [BsonElement("LibraryId")]
        public ObjectId LibraryId { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}

