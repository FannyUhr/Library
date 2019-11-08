using DataInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class BorrowManager : IBorrowManager
    {
        public Borrow AddBorrow(int customerNumber, string bookTitle, DateTime borrowDate, DateTime returnDate)
        {
            using var libraryContext = new LibraryContext();
            var customer = (from c in libraryContext.Customers
                            where c.CustomerNumber == customerNumber
                            select c).First();

            var book = (from b in libraryContext.Books
                        where b.BookTitle == bookTitle                         
                        select b).First();

            var borrow = new Borrow();
            borrow.CustomerID = customer.CustomerID;
            borrow.BookID = book.BookID;
            borrow.BorrowDate = borrowDate;
            borrow.ReturnDate = returnDate;
            libraryContext.Borrows.Add(borrow);
            libraryContext.SaveChanges();
            return borrow;
        }

        public void AddBorrowToBook(string bookTitle, bool isBorrowed, Borrow borrow)
        {
            using var libraryContext = new LibraryContext();
            libraryContext.Borrows.Attach(borrow);
            var book = (from b in libraryContext.Books
                         where b.BookTitle == bookTitle
                         select b).First();
            book.IsBorrowed = isBorrowed;
            book.Borrows.Add(borrow);
            libraryContext.SaveChanges();
        }

        public void AddToHistoryListOfBorrow(int customerNumber, string bookTitle, DateTime borrowDate)
        {
            using var libraryContext = new LibraryContext();            
            var historyBorrowList = new HistoryBorrowList();
            historyBorrowList.CustomerNumber = customerNumber;
            historyBorrowList.BookTitle = bookTitle;
            historyBorrowList.BorrowDate = borrowDate;
            libraryContext.HistoryBorrowLists.Add(historyBorrowList);
            libraryContext.SaveChanges();
        }

        public void MoveShelf(int shelfID, int aisleID)
        {
            using var libraryContext = new LibraryContext();
            var shelf = (from s in libraryContext.Shelfs
                         where s.ShelfID == shelfID
                         select s)
                         .First();
            shelf.AisleID = aisleID;
            libraryContext.SaveChanges();
        }
        public void ExtendBorrowDate(int borrowID, DateTime borrowDate, DateTime returnDate)
        {
            using var libraryContext = new LibraryContext();
            var borrow = (from b in libraryContext.Borrows
                          where b.BorrowID == borrowID
                          select b)
                         .First();
            borrow.BorrowDate = borrowDate;
            borrow.ReturnDate = returnDate;
            libraryContext.SaveChanges();
        }

        public void ReturnBorrow(int borrowID, Book book, int condition)
        {
            using var libraryContext = new LibraryContext();
            var borrow = (from b in libraryContext.Borrows
                        where b.BorrowID == borrowID
                        select b).FirstOrDefault();
            book.Condition = condition;
            libraryContext.Borrows.Remove(borrow);
            libraryContext.SaveChanges();
        }

        public void CreateInvoice(Customer customer, Borrow borrow, decimal invoiceSum, decimal debt)
        {
            using var libraryContext = new LibraryContext();
            var invoice = new Invoice();
            invoice.CustomerID = customer.CustomerID;
            invoice.BorrowID = borrow.BorrowID;
            invoice.InvoiceSum = invoiceSum;
            customer.Debt = debt;
            libraryContext.Invoices.Add(invoice);
            libraryContext.SaveChanges();
        }

        public void RemoveInvoice(Invoice invoice, decimal invoiceSum)
        {
            using var libraryContext = new LibraryContext();
            invoice.Customer.Debt = invoice.Customer.Debt - invoiceSum;
            libraryContext.Invoices.Remove(invoice);
            libraryContext.SaveChanges();
        }
        
        public Customer GetCustomerByBorrow(Borrow borrow)
        {
            using var libraryContext = new LibraryContext();
            var borrows = (from b in libraryContext.Borrows
                          where b.BorrowID == borrow.BorrowID
                          select b).First();

            return (from c in libraryContext.Customers
                    join b in libraryContext.Borrows
                    on c.CustomerID equals b.CustomerID
                    where b.BorrowID == borrows.BorrowID
                    select c).FirstOrDefault();
        }
        
        public Borrow GetBorrowFromCustomerAndBook(int customerNumber, string bookTitle)
        {
            using var libraryContext = new LibraryContext();
            var customer = (from c in libraryContext.Customers
                            where c.CustomerNumber == customerNumber
                            select c).First();
            
            var book = (from b in libraryContext.Books
                            where b.BookTitle == bookTitle
                            select b).First();

            return (from c in libraryContext.Customers
                    join b in libraryContext.Borrows
                    on c.CustomerID equals b.CustomerID
                    join bk in libraryContext.Books
                    on b.BookID equals bk.BookID
                    where b.CustomerID == customer.CustomerID && b.BookID == book.BookID
                    select b).FirstOrDefault();
        }                

        public List<Borrow> GetAllCurrentBorrowFromCustomer(int customerNumber)
        {
            using var libraryContext = new LibraryContext();
            var customer = (from c in libraryContext.Customers
                            where c.CustomerNumber == customerNumber
                            select c).First();

            return (from b in libraryContext.Borrows
                    where b.CustomerID == customer.CustomerID
                    select b).ToList();
        }

        public List<Borrow> GetBorrowWithOverdueReturnDate(DateTime todaysDate)
        {
            using var libraryContext = new LibraryContext();
            return (from b in libraryContext.Borrows
                    where b.ReturnDate >= todaysDate
                    select b)
                    .Include(b => b.Customer.Debt)
                    .ToList();                    
        }
        
        public Invoice GetInvoiceByBorrow(Borrow borrow)
        {
            using var libraryContext = new LibraryContext();
            return (from i in libraryContext.Invoices
                    where i.BorrowID == borrow.BorrowID
                    select i).FirstOrDefault();
        }        
    }
}
