using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public interface IBookManager
    {
        Book AddBook(string iSBNNumber, string bookTitle, string author, int purchaseYear, int price, int condition, Shelf shelf,
            bool isBorrowed, Borrow borrow);
        void MoveBook(int bookID, int shelfID);
        void RemoveBook(int bookID);
        Book GetBookByBookTitle(string bookTitle);
        Shelf GetShelfFromBook(Book book);
        void CreateDiscardList(Shelf shelf, Book book);
        void ClearDiscardList(List<Book> book);        
        List<Book> GetAllBooksInDiscardList();
        List<Book> GetAllBooksWithConditionOne(int condition, bool isBorrowed);
        Discard GetBookFromDiscardByBook(Book book);        
    }
}
