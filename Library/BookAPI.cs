using DataInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Library
{
    public class BookAPI
    {
        IShelfManager shelfManager;
        IBookManager bookManager;

        public BookAPI(IShelfManager shelfManager, IBookManager bookManager)
        {
            this.shelfManager = shelfManager;
            this.bookManager = bookManager;
        }

        public AddBookStatusCodes AddBook(string iSBNNumber, string bookTitle, string author, int price, int condition,
            int shelfNumber, int aisleNumber)
        {
            var existingShelf = shelfManager.GetShelfFromAisle(shelfNumber, aisleNumber);
            var purchaseYear = DateTime.Today.Year;
           
            if (existingShelf == null)
                return AddBookStatusCodes.NoSuchShelf;
            
            bookManager.AddBook(iSBNNumber, bookTitle, author, purchaseYear, price, condition, existingShelf, false, null);
            return AddBookStatusCodes.Ok;
        }

        public MoveBookStatusCodes MoveBook(string bookTitle, int shelfNumber, int aisleNumber)
        {
            var book = bookManager.GetBookByBookTitle(bookTitle);            
            if (book == null)
                return MoveBookStatusCodes.NoSuchBook;
            var newShelf = shelfManager.GetShelfFromAisle(shelfNumber, aisleNumber);
            if (newShelf == null)
                return MoveBookStatusCodes.NoSuchShelf;
            if (book.Shelf.ShelfID == newShelf.ShelfID)
                return MoveBookStatusCodes.BookAlreadyInThatShelf;
                        
            bookManager.MoveBook(book.BookID, newShelf.ShelfID);
            return MoveBookStatusCodes.Ok;            
        }

        public RemoveBookStatusCodes RemoveBook(string bookTitle)
        {
            var book = bookManager.GetBookByBookTitle(bookTitle);
            if (book == null)
                return RemoveBookStatusCodes.NoSuchBook;
            if (book.IsBorrowed == true)
                return RemoveBookStatusCodes.BookIsBorrowed;

            bookManager.RemoveBook(book.BookID);
            return RemoveBookStatusCodes.Ok;
        }

        public CreateDiscardListStatusCodes CreateDiscardList()
        {
            int condition = 1;
            bool isBorrowed = false;
            var newDiscardList = new List<Book>(bookManager.GetAllBooksWithConditionOne(condition, isBorrowed));
            if (newDiscardList.Count == 0)
                return CreateDiscardListStatusCodes.NoBooksInConditionOne;
            for (int i = 0; i < newDiscardList.Count; i++)
            {
                var existingDiscardBook = bookManager.GetBookFromDiscardByBook(newDiscardList[i]);
                if (existingDiscardBook == null)
                {
                    var shelf = bookManager.GetShelfFromBook(newDiscardList[i]);
                    bookManager.CreateDiscardList(shelf, newDiscardList[i]);
                }                
            }
            return CreateDiscardListStatusCodes.Ok;
        }

        public ClearDiscardListStatusCodes ClearDiscardList()
        {
            var allDiscardBooks = bookManager.GetAllBooksInDiscardList();
            if (allDiscardBooks.Count == 0)
                return ClearDiscardListStatusCodes.NoDiscardBooksToClear;

            bookManager.ClearDiscardList(allDiscardBooks);
            return ClearDiscardListStatusCodes.Ok;
        }
    }
}
