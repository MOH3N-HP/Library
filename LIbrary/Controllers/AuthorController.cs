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
    public class AuthorController : Controller
    {
        public async Task<IActionResult> Add()
        {
            var ViewModel = await AuthorServices.ReturnAddViewModel();
            return View(ViewModel);
        }

        public async Task<IActionResult> Update(string authorId)
        {
            var ViewModel = await AuthorServices.ReturnUpdateViewModel(authorId);
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Author author, string BookName)
        {

            if (author.Name == null)
            {
                ModelState.AddModelError("Author.Name", "The author should have a name.");
                var ViewModel = await AuthorServices.ReturnAddViewModel();
                return View(ViewModel);
            }
            if (author.Age == 0)
            {
                ModelState.AddModelError("Author.Age", "The author should be born.");
                var ViewModel = await AuthorServices.ReturnAddViewModel();
                return View(ViewModel);
            }
            var authorsfilter = Builders<Author>.Filter.Eq(a => a.Name, author.Name);
            var authoroperations = new Operations<Author>();
            if ((await authoroperations.FindMany(authorsfilter)).Count > 0)
            {
                ModelState.AddModelError("Author.Name", "This author already exists.");
                var ViewModel = await AuthorServices.ReturnAddViewModel();
                return View(ViewModel);
            }
            
            await AuthorServices.AddNewAuthor(author, BookName);
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List(string userInput)
        {
            var ViewModels = await AuthorServices.ReturnAuthorsList(userInput);
            return View(ViewModels);
        }

        [HttpGet("SearchAuthorsAjax")]
        public async Task<IActionResult> SearchAuthorsAjax([FromQuery] string userInput)
        {
            List<AuthorListViewModel> viewModels = await AuthorServices.ReturnAuthorsList(userInput);
            return Json(viewModels);
        }

        [HttpDelete("Author/Delete/{authorId}")]
        public async Task<IActionResult> Delete(string authorId)
        {
            var IsAcknowledged = await AuthorServices.DeleteAuthor(authorId);
            if (IsAcknowledged)
            {
                return Ok();
            }
            return BadRequest("Failed");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string Id, string Name, int Age, string BookName)
        {
            if (Id == null)
            {
                return RedirectToAction("Update", new { authorId = Id });
            }
           
            if (Name == null && Age == 0 &&  BookName == null)
            {
                return RedirectToAction("Update", new { authorId = Id });
            }

            await AuthorServices.UpdateAuthor(Id, Name, Age, BookName);
            return RedirectToAction("List", new {userInput = Id});
        }

        [HttpGet]
        public async Task<IActionResult> RemoveBook(ObjectId AuthorId, ObjectId bookId)
        {
            await AuthorServices.RemoveBook(AuthorId, bookId);
            var author = await AuthorServices.Find(AuthorId.ToString());
            return RedirectToAction("Update", new { authorId = author[0].Id.ToString()});
        }
    }
}
