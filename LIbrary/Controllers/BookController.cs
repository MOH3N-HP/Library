using LIbrary.Command;
using LIbrary.Models;
using LIbrary.Services;
using LIbrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LIbrary.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        public async Task<IActionResult> Add()
        {
            var ViewModel = await BookServices.ReturnAddViewModel();
            return View(ViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string bookId)
        {
            var ViewModel = await BookServices.ReturnUpdateViewModel(bookId);
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Book book, string AuthorName, string LibraryName)
        {
            if (book.Title == null)
            {
                ModelState.AddModelError("Book.Title", "This book should have a title.");
                var ViewModel = await BookServices.ReturnAddViewModel();
                return View(ViewModel);
            }
            if (book.Description == null)
            {
                ModelState.AddModelError("Book.Description", "This book should be described.");
                var ViewModel = await BookServices.ReturnAddViewModel();
                return View(ViewModel);

            }

            var filter = Builders<Book>.Filter.Eq(b => b.Title, book.Title);
            var bookoperations = new Operations<Book>();
            if ((await bookoperations.FindMany(filter)).Count > 0)
            {
                ModelState.AddModelError("Book.Title", "This book already exists.");
                var ViewModel = await BookServices.ReturnAddViewModel();
                return View(ViewModel);
            }

            await BookServices.AddNewBook(book, AuthorName, LibraryName);
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List(string userInput)
        {
            var ViewModels = await BookServices.ReturnBooksList(userInput);
            return View(ViewModels);
        }

        [HttpGet("SearchBooksAjax")]
        public async Task<IActionResult> SearchBooksAjax([FromQuery] string userInput)
        {
            List<BookListViewModel> viewModels = await BookServices.ReturnBooksList(userInput);
            return Json(viewModels);
        }

        [HttpDelete("Book/Delete/{bookId}")]
        public async Task<IActionResult> Delete(string bookId)
        {
            var IsAcknowledged = await BookServices.DeleteBook(bookId);
            if (IsAcknowledged)
            {
                return Ok();
            }
            return BadRequest("Failed the request.");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string Id,string Title, string Description, string LibraryName, string AuthorName)
        {
            if (Id == null)
            {
                return RedirectToAction("Update", new { bookId = Id });
            }

            if (Title == null && Description == null && LibraryName == null && AuthorName == null)
            {
                return RedirectToAction("Update", new { bookId = Id});
            }

            await BookServices.UpdateBook(Id, Title, Description, LibraryName, AuthorName);
            return RedirectToAction("List", new { userInput = Id });
        }

        [HttpGet]
        public async Task<IActionResult> RemoveAuthor(ObjectId BookId, ObjectId authorId)
        {
            await BookServices.RemoveAuthor(BookId, authorId);
            var book = await BookServices.Find(BookId.ToString());
            return RedirectToAction("Update", new { bookId = book[0].Id.ToString() });
        }
    }
}
