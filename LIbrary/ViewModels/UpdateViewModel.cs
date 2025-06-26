using LIbrary.Models;

namespace LIbrary.ViewModels
{
    public class UpdateViewModel
    {
        public string Id { get; set; } = null!;
        public Author? Author { get; set; }
        public Book? Book { get; set; }
        public List<Book> AllBooks { get; set; }
        public List<Author> AllAuthors { get; set; }
        public List<Library> AllLibraries { get; set; }
        public List<Book> ThisBooks { get; set; }
        public List<Author> ThisAuthors { get; set; }
    }
}
