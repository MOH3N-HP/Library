using MongoDB.Driver;

namespace LIbrary.Command
{
    public class Commands
    {
        public static readonly string _connectionString = "mongodb://localhost:27017";
        public static IMongoDatabase GetMongoDatabase()
        {
            var _client = new MongoClient(_connectionString);
            return _client.GetDatabase("Library_Database");
        }
    }
}
