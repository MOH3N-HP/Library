﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LIbrary.Models
{
    public class Library
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;
    }
}
