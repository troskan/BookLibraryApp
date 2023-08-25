using BookLibraryApi.Models;

namespace BookLibraryApp.ViewModels
{
    public class BookEditViewModel
    {
        public Book Book { get; set; }
        public List<string> Genres { get; set; }
    }
}
