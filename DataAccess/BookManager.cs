using DataInterface;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class BookManager : IBookManager
    {
        public Book AddBook(string iSBNNumber, string bookTitle, string author, int purchaseYear, int price, int condition, Shelf shelf, 
            bool isBorrowed, Borrow borrow)
        {
            using var libraryContext = new LibraryContext();
            libraryContext.Borrows.Attach(borrow);
            var book = new Book();
            book.ISBNNumber = iSBNNumber;
            book.BookTitle = bookTitle;
            book.Author = author;
            book.PurchaseYear = purchaseYear;
            book.Condition = condition;
            book.Price = price;            
            book.ShelfID = shelf.ShelfID;
            book.IsBorrowed = isBorrowed;
            book.Borrows.Add(borrow);
            
            libraryContext.Books.Add(book);
            libraryContext.SaveChanges();
            return book;
        }

        public void MoveBook(int bookID, int shelfID)
        {
            using var libraryContext = new LibraryContext();
            var book = (from b in libraryContext.Books
                        where b.BookID == bookID
                        select b)
                         .First();
            book.ShelfID = shelfID;
            libraryContext.SaveChanges();
        }

        public void RemoveBook(int bookID)
        {
            using var libraryContext = new LibraryContext();
            var book = (from b in libraryContext.Books
                        where b.BookID == bookID
                        select b).FirstOrDefault();
            libraryContext.Books.Remove(book);
            libraryContext.SaveChanges();
        }

        public Book GetBookByBookTitle(string bookTitle)
        {
            using var libraryContext = new LibraryContext();
            return (from b in libraryContext.Books
                    where b.BookTitle == bookTitle
                    select b).FirstOrDefault();
        }
                
        public void CreateDiscardList(Shelf shelf, Book book)
        {
            using var libraryContext = new LibraryContext();
            var discard = new Discard();
            discard.ShelfID = shelf.ShelfID;
            discard.BookID = book.BookID;
            libraryContext.Discards.Add(discard);
            libraryContext.SaveChanges();
        }

        public void ClearDiscardList(List<Book> book)
        {
            using var libraryContext = new LibraryContext();
            var discardData = (from d in libraryContext.Discards
                               select d).ToList();         

            libraryContext.Books.RemoveRange(book);
            libraryContext.Discards.RemoveRange(discardData);
            libraryContext.SaveChanges();
        }

        public List<Book> GetAllBooksWithConditionOne(int condition, bool isBorrowed)
        {
            using var libraryContext = new LibraryContext();
            return (from b in libraryContext.Books
                    where b.Condition == condition && b.IsBorrowed == isBorrowed
                    select b).ToList();
        }

        public List<Book> GetAllBooksInDiscardList()
        {
            using var libraryContext = new LibraryContext();
            return (from d in libraryContext.Discards
                    join b in libraryContext.Books
                    on d.BookID equals b.BookID
                    where d.BookID == b.BookID
                    select b).ToList();
        }

        public Discard GetBookFromDiscardByBook(Book book)
        {
            using var libraryContext = new LibraryContext();
            return (from d in libraryContext.Discards
                    where d.BookID == book.BookID
                    select d).FirstOrDefault();
        }

        public Shelf GetShelfFromBook(Book book)
        {
            using var libraryContext = new LibraryContext();
            return (from s in libraryContext.Shelfs
                    join b in libraryContext.Books
                    on s.ShelfID equals book.ShelfID
                    where s.ShelfID == book.ShelfID
                    select s).FirstOrDefault();
        }        
    }
}
