using LIbrary.Command;
using LIbrary.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LIbrary.Services
{
    public class LibraryServices
    {

        private readonly static Operations<Library> libraryoperations = new("Libraries");

        public async static Task<string> GetLibraryName(ObjectId libraryId)
        {
            var filter = Builders<Library>.Filter.Eq(l => l.Id, libraryId);
            var library = await libraryoperations.Find(filter);
            if (library == null)
            {
                return "";
            }
            return library.Name;
        }
    }
}
