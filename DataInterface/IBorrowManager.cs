using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public interface IBorrowManager
    {
        Borrow AddBorrow(int customerNumber, string bookTitle, DateTime borrowDate, DateTime returnDate);
        void AddBorrowToBook(string bookTitle, bool isBorrowed, Borrow borrow);
        void AddToHistoryListOfBorrow(int customerNumber, string bookTitle, DateTime borrowDate);
        void ExtendBorrowDate(int borrowID, DateTime borrowDate, DateTime returnDate);
        void ReturnBorrow(int borrowID, Book book, int condition);        
        Borrow GetBorrowFromCustomerAndBook(int customerNumber, string bookTitle);           
        List<Borrow> GetAllCurrentBorrowFromCustomer(int customerNumber);
        List<Borrow> GetBorrowWithOverdueReturnDate(DateTime todaysDate);
        Customer GetCustomerByBorrow(Borrow borrow);        
        void CreateInvoice(Customer customer, Borrow borrow, decimal invoiceSum, decimal debt);
        void RemoveInvoice(Invoice invoice, decimal invoiceSum);
        Invoice GetInvoiceByBorrow(Borrow borrow);
    }
}
