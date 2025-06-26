namespace LIbrary.ViewModels
{
    public class BookListViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LibraryName { get; set; }
        public List<string> Authors { get; set; }
    }
}
