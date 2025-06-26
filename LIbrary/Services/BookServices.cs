using LIbrary.Command;
using LIbrary.Models;
using LIbrary.ViewModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LIbrary.Services
{
    public class BookServices
    {
        private readonly static Operations<Author> authoroperations = new();
        private readonly static Operations<Book> bookoperations = new();
        private readonly static Operations<Library> libraryoperations = new("Libraries");

        public async static Task AddNewBook(Book book, string AuthorName, string LibraryName)
        {
            if (AuthorName != null)
            {
                var authorsfilter = Builders<Author>.Filter.Eq(a => a.Name, AuthorName);
                var author = await authoroperations.Find(authorsfilter);
                book.Authors.Add(author.Id);
            }
            if (LibraryName != null)
            {
                var authorsfilter = Builders<Library>.Filter.Eq(a => a.Name, LibraryName);
                var library = await libraryoperations.Find(authorsfilter);
                book.LibraryId = library.Id;
            }

            await bookoperations.AddOne(book);
        }

        public async static Task UpdateBook(string Id, string Title, string Description, string LibraryName, string AuthorName)
        {
            var objectId = ObjectId.Parse(Id);
            var filter = Builders<Book>.Filter.Eq(a => a.Id, objectId);
            var updated = false;
            UpdateDefinition<Book> update;
            if (Title != null)
            {
                update = Builders<Book>.Update.Set(a => a.Title, Title);
                updated = await bookoperations.Update(filter, update);
            }
            if (Description != null)
            {
                update = Builders<Book>.Update.Set(a => a.Description, Description);
                updated = await bookoperations.Update(filter, update);
            }
            if (LibraryName != null)
            {
                var filt = Builders<Library>.Filter.Eq(l => l.Name, LibraryName);
                var library = await libraryoperations.Find(filt);
                update = Builders<Book>.Update.Set(a => a.LibraryId, library.Id);
                updated = await bookoperations.Update(filter, update);
            }
            if (AuthorName != null)
            {
                var filt = Builders<Author>.Filter.Eq(a => a.Name, AuthorName);
                var author = await authoroperations.Find(filt);
                update = Builders<Book>.Update.Push(a => a.Authors, author.Id);
                updated = await bookoperations.Update(filter, update);
            }
        }

        public async static Task<List<Book>> Find(string userInput)
        {
            var builder = Builders<Book>.Filter;
            FilterDefinition<Book> filter;
            if (ObjectId.TryParse(userInput, out ObjectId objectId))
            {
                filter = builder.Eq(b => b.Id, objectId);
            }
            else
            {
                filter = builder.Eq(b => b.Title, userInput);
            }
            var books = await bookoperations.FindMany(filter);
            return books;
        }

        public async static Task<bool> DeleteBook(string bookId)
        {
            var id = ObjectId.Parse(bookId);
            var filter = Builders<Book>.Filter.Eq(b => b.Id, id);
            var IsAcknowledged = await bookoperations.Delete(filter);
            return IsAcknowledged;
        }

        public async static Task<string> GetBookName(ObjectId tmpId)
        {
            var bookId = tmpId.ToString();
            var books = await Find(bookId);
            if (books.Count == 0)
            {
                return "";
            }
            return books[0].Title;
        }

        public async static Task AddAuthor(ObjectId BookId, ObjectId authorId)
        {
            var author = await Find(BookId.ToString());
            var filter = Builders<Book>.Filter.Eq(b => b.Id, BookId);
            var add = Builders<Book>.Update.Push(b => b.Authors, authorId);

            var updated = await bookoperations.Update(filter, add);
        }

        public async static Task RemoveAuthor(ObjectId BookId, ObjectId authorId)
        {
            var author = await Find(BookId.ToString());
            var filter = Builders<Book>.Filter.Eq(b => b.Id, BookId);
            var delete = Builders<Book>.Update.Pull(b => b.Authors, authorId);

            var updated = await bookoperations.Update(filter, delete);
        }

        public async static Task<AddViewModel> ReturnAddViewModel()
        {
            var auhtorsfilter = Builders<Author>.Filter.Empty;
            var authors = await authoroperations.FindMany(auhtorsfilter);
            var librariesfilter = Builders<Library>.Filter.Empty;
            var libraries = await libraryoperations.FindMany(librariesfilter);
            var ViewModel = new AddViewModel
            {
                Authors = authors,
                Librarys = libraries
            };

            return ViewModel;
        }

        public async static Task<UpdateViewModel> ReturnUpdateViewModel(string bookId)
        {
            var book = await Find(bookId);
            var filter = Builders<Author>.Filter.Empty;
            var filter2 = Builders<Library>.Filter.Empty;
            var authors = await authoroperations.FindMany(filter);
            var libraries = await libraryoperations.FindMany(filter2);
            List<Author> thisAuthors = [];
            foreach (var authorId in book[0].Authors)
            {
                var strid = authorId.ToString();
                var author = await AuthorServices.Find(strid);
                if (author.Count > 0)
                    thisAuthors.Add(author[0]);
            }
            var viewModel = new UpdateViewModel
            {
                Id = bookId,
                Book = book[0],
                AllAuthors = authors,
                AllLibraries = libraries,
                ThisAuthors = thisAuthors
            };
            return viewModel;
        }

        public async static Task<List<BookListViewModel>> ReturnBooksList(string userInput)
        {
            List<Book> books;
            List<BookListViewModel> models = [];
            if (userInput == null)
            {
                var filter = Builders<Book>.Filter.Empty;
                books = await bookoperations.FindMany(filter);
            }
            else
            {
                books = await Find(userInput);
            }

            foreach(var book in books)
            {
                List<string> authors = [];
                foreach(var author in book.Authors)
                {
                    authors.Add(await AuthorServices.GetAuthorName(author));
                }
                var viewmodel = new BookListViewModel
                {
                    Id = book.Id.ToString(),
                    Title = book.Title,
                    Description = book.Description,
                    LibraryName = await LibraryServices.GetLibraryName(book.LibraryId),
                    Authors = authors
                };
                models.Add(viewmodel);
            }

            return models;
        }
    }
}
