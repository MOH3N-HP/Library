using LIbrary.Models;

namespace LIbrary.ViewModels
{
    public class AddViewModel
    {
        public Author Author { get; set; }
        public Book Book { get; set; }
        public List<Book> Books { get; set; }
        public List<Author> Authors { get; set; }
        public List<Library> Librarys { get; set; }
    }
}
