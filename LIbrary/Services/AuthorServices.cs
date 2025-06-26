using LIbrary.Command;
using LIbrary.Models;
using LIbrary.ViewModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LIbrary.Services
{
    public class AuthorServices
    {

        private readonly static Operations<Author> authoroperations = new();
        private readonly static Operations<Book> bookoperations = new();

        public async static Task AddNewAuthor(Author author, string BookName)
        {
            if (BookName != null)
            {
                var booksfilter = Builders<Book>.Filter.Eq(b => b.Title, BookName);
                var book = await bookoperations.Find(booksfilter);
                author.Books.Add(book.Id);
            }
            await authoroperations.AddOne(author);
        }

        public async static Task UpdateAuthor(string Id, string Name, int Age, string BookName)
        {
            var objectId = ObjectId.Parse(Id);
            var filter = Builders<Author>.Filter.Eq(a => a.Id, objectId);
            var updated = false;
            UpdateDefinition<Author> update;
            if (Name != null)
            {
                update = Builders<Author>.Update.Set(a => a.Name, Name);
                updated = await authoroperations.Update(filter, update);
            }
            if (Age > 0)
            {
                update = Builders<Author>.Update.Set(a => a.Age, Age);
                updated = await authoroperations.Update(filter, update);
            }
            if (BookName != null)
            {
                var fil = Builders<Book>.Filter.Eq(b => b.Title, BookName);
                var book = await bookoperations.Find(fil);
                ObjectId id = book.Id;
                update = Builders<Author>.Update.Push(a => a.Books, id);
                updated = await authoroperations.Update(filter, update);
            }
        }

        public async static Task<List<Author>> Find(string input)
        {
            var builder = Builders<Author>.Filter;
            FilterDefinition<Author> filter;
            if (ObjectId.TryParse(input, out ObjectId objectId))
            {
                filter = builder.Eq(b => b.Id, objectId);
            }
            else
            {
                filter = builder.Eq(b => b.Name, input);
            }
            var authors = await authoroperations.FindMany(filter);
            return authors;
        }
        public async static Task<string> GetAuthorName(ObjectId Id)
        {
            var authorId = Id.ToString();
            var authors = await Find(authorId);
            if (authors.Count == 0)
            {
                return "";
            }
            return authors[0].Name;
        }

        public async static Task<bool> DeleteAuthor(string authorId)
        {
            var id = ObjectId.Parse(authorId);
            var filter = Builders<Author>.Filter.Eq(b => b.Id, id);
            var IsAcknowledged = await authoroperations.Delete(filter);

            return IsAcknowledged;
        }

        public async static Task AddBook(ObjectId AuthorId, ObjectId bookId)
        {
            var author = await Find(AuthorId.ToString());
            var filter = Builders<Author>.Filter.Eq(a => a.Id, AuthorId);
            var add = Builders<Author>.Update.Push(a => a.Books, bookId);

            var updated = await authoroperations.Update(filter, add);
        }

        public async static Task RemoveBook(ObjectId AuthorId, ObjectId bookId)
        {
            var author = await Find(AuthorId.ToString());
            var filter = Builders<Author>.Filter.Eq(a => a.Id, AuthorId);
            var delete = Builders<Author>.Update.Pull(a => a.Books, bookId);

            var updated = await authoroperations.Update(filter, delete);
        }

        public async static Task<AddViewModel> ReturnAddViewModel()
        {
            var filter = Builders<Book>.Filter.Empty;
            var books = await bookoperations.FindMany(filter);
            var ViewModel = new AddViewModel
            {
                Books = books,
            };

            return ViewModel;
        }

        public async static Task<UpdateViewModel> ReturnUpdateViewModel(string authorId)
        {
            var filter = Builders<Book>.Filter.Empty;
            var author = await Find(authorId);
            var allBooks = await bookoperations.FindMany(filter);
            List<Book> thisBooks = [];
            foreach (var bookid in author[0].Books)
            {
                var strid = bookid.ToString();
                var book = await BookServices.Find(strid);
                thisBooks.Add(book[0]);
            }
            var ViewModel = new UpdateViewModel
            {
                Id = authorId,
                Author = author[0],
                AllBooks = allBooks,
                ThisBooks = thisBooks
            };

            return ViewModel;
        }

        public async static Task<List<AuthorListViewModel>> ReturnAuthorsList(string userInput)
        {
            List<Author> authors;
            List<AuthorListViewModel> models = [];
            if (userInput == null)
            {
                var filter = Builders<Author>.Filter.Empty;
                authors = await authoroperations.FindMany(filter);
            }
            else
            {
                authors = await Find(userInput);
            }

            foreach (var author in authors)
            {
                List<string> books = [];
                foreach (var book in author.Books)
                {
                    books.Add(await BookServices.GetBookName(book));
                }
                var ViewModel = new AuthorListViewModel
                {
                    Id = author.Id.ToString(),
                    Name = author.Name,
                    Age = author.Age,
                    Books = books
                };
                models.Add(ViewModel);
            }

            return models;
        }
    }
}
