using BookMan.WebApp.Interface;
using BookMan.WebApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookMan.WebApp.Pages
{
    public class BookModel : PageModel
    {
        private readonly IRepository _repository;
        public BookModel(IRepository repository)
        {
            _repository = repository;
        }
        public enum Action
        {
            Detail, Delete, Update, Create
        }
        public Action Job { get; set; }
        public Book Book { get; set; }
        public void OnGet(int id)
        {
            Job = Action.Detail;
            Book = _repository.Get(id);
            ViewData["Title"] = Book == null ? "Not found" : $"Detail - {Book.Title}";
        }

        public void OnGetDelete(int id)
        {
            Job = Action.Delete;
            Book = _repository.Get(id);
            ViewData["Title"] = Book == null ? "Not found" : $"Confirm delete: {Book.Title}";
        }
        public IActionResult OnGetConfirm(int id)
        {
            _repository.Delete(id);
            return new RedirectToPageResult("index");
        }
        public void OnGetCreate()
        {
            Job = Action.Create;
            Book = _repository.Create();
            ViewData["Title"] = "Create a new book";
        }
        public IActionResult OnPostCreate(Book book)
        {
            _repository.Add(book);
            return new RedirectToPageResult("index");
        }
        public void OnGetUpdate(int id)
        {
            Job = Action.Update;
            Book = _repository.Get(id);
            ViewData["Title"] = Book == null ? "Not found" : $"Updating: {Book.Title}";
        }
        public IActionResult OnPostUpdate(Book book)
        {
            _repository.Update(book);
            return new RedirectToPageResult("index");
        }
    }
}
