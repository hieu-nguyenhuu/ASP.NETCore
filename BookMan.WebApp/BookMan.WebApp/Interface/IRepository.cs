using System.Collections.Generic;
using BookMan.WebApp.Model;
namespace BookMan.WebApp.Interface
{
    public interface IRepository
    {
        public HashSet<Book> Books { get; set; }
        public Book Get(int id);
        public bool Delete(int id);
        public Book Create();
        public bool Add(Book book);
        public bool Update(Book book);
    }
}