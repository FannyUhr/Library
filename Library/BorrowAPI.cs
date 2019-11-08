using DataInterface;
using System;

namespace Library
{
    public class BorrowAPI
    {
        private IBorrowManager borrowManager;
        private ICustomerManager customerManager;
        private IBookManager bookManager;

        public BorrowAPI(IBorrowManager borrowManager, ICustomerManager customerManager, IBookManager bookManager)
        {
            this.borrowManager = borrowManager;
            this.customerManager = customerManager;
            this.bookManager = bookManager;
        }

        public AddBorrowStatusCodes AddBorrow(int customerNumber, string bookTitle)
        {                        
            var newBorrowDate = DateTime.Today;
            var newReturnDate = DateTime.Today.AddDays(30);
            
            var customer = customerManager.GetCustomerByCustomerNumber(customerNumber);
            if (customer == null)
                return AddBorrowStatusCodes.NoSuchCustomer;
            var book = bookManager.GetBookByBookTitle(bookTitle);
            if (book == null)
                return AddBorrowStatusCodes.NoSuchBook;
            var newBorrow = borrowManager.GetBorrowFromCustomerAndBook(customerNumber, bookTitle);
            if (book.IsBorrowed == true)            
            {
                if (newBorrow != null && customer.Debt == 0)
                {
                    borrowManager.ExtendBorrowDate(newBorrow.BorrowID, newBorrowDate, newReturnDate);
                    borrowManager.AddToHistoryListOfBorrow(customerNumber, bookTitle, newBorrowDate);
                    return AddBorrowStatusCodes.ExtendBorrowForCustomer;
                }
                else if (newBorrow != null && customer.Debt > 0)                
                    return AddBorrowStatusCodes.CustomerHasDebtCanNotExtendBorrow;
                                
                return AddBorrowStatusCodes.BookIsUnavailable;
            }
            if (borrowManager.GetAllCurrentBorrowFromCustomer(customerNumber).Count >= 5)
                return AddBorrowStatusCodes.CustomerCanNotExceedMaxBookQuantity;
            if (customer.Debt > 0)
                return AddBorrowStatusCodes.CustomerHasDebt;
            
            borrowManager.AddBorrow(customerNumber, bookTitle, newBorrowDate, newReturnDate);
            borrowManager.AddBorrowToBook(bookTitle, true, newBorrow);
            borrowManager.AddToHistoryListOfBorrow(customerNumber, bookTitle, newBorrowDate);
            return AddBorrowStatusCodes.Ok;                    
        }

        public ReturnBorrowStatusCodes ReturnBorrow(int customerNumber, string bookTitle, int condition)
        {
            var customer = customerManager.GetCustomerByCustomerNumber(customerNumber);
            if (customer == null)
                return ReturnBorrowStatusCodes.NoSuchCustomer;
            var book = bookManager.GetBookByBookTitle(bookTitle);
            if (book == null)
                return ReturnBorrowStatusCodes.NoSuchBook;
            var borrow = borrowManager.GetBorrowFromCustomerAndBook(customerNumber, bookTitle);
            if (borrow == null)
                return ReturnBorrowStatusCodes.NoSuchBorrow;

            borrowManager.ReturnBorrow(borrow.BorrowID, book, condition);
            return ReturnBorrowStatusCodes.Ok;
        }

        public CreateInvoiceStatusCodes CreateInvoice()
        {
            var todaysDate = DateTime.Today;
            var borrowOverdueReturnDate = borrowManager.GetBorrowWithOverdueReturnDate(todaysDate);
            decimal overdueCharge = 30;
            if (borrowOverdueReturnDate.Count == 0)
                return CreateInvoiceStatusCodes.NoBorrowOverdueDate;

            for (int i = 0; i < borrowOverdueReturnDate.Count; i++)
            {
                var existingInvoice = borrowManager.GetInvoiceByBorrow(borrowOverdueReturnDate[i]);
                var customer = borrowManager.GetCustomerByBorrow(borrowOverdueReturnDate[i]);
                decimal totalDebt = customer.Debt + overdueCharge;
                if (existingInvoice == null && borrowOverdueReturnDate[i].ReturnDate.CompareTo(todaysDate) < 30)
                {
                    borrowManager.CreateInvoice(customer, borrowOverdueReturnDate[i], overdueCharge, totalDebt);
                }                                                                      
                if (existingInvoice == null && borrowOverdueReturnDate[i].ReturnDate.CompareTo(todaysDate) > 30)
                {
                    borrowManager.CreateInvoice(customer, borrowOverdueReturnDate[i], (overdueCharge * 2), (totalDebt + overdueCharge));
                }                                    
                if (existingInvoice != null && existingInvoice.Borrow.ReturnDate.CompareTo(todaysDate) < 30)
                    continue;
                if (existingInvoice != null && existingInvoice.Borrow.ReturnDate.CompareTo(todaysDate) > 30)
                {
                    borrowManager.RemoveInvoice(existingInvoice, existingInvoice.InvoiceSum);
                    borrowManager.CreateInvoice(customer, borrowOverdueReturnDate[i], (overdueCharge * 2), (totalDebt + overdueCharge));
                }
                if (existingInvoice != null && existingInvoice.Borrow.ReturnDate.CompareTo(todaysDate) > 60)
                {
                    borrowManager.RemoveInvoice(existingInvoice, existingInvoice.InvoiceSum);
                    borrowManager.CreateInvoice(customer, borrowOverdueReturnDate[i], (overdueCharge * 3), (totalDebt + (overdueCharge * 2)));
                }
                if (existingInvoice != null && existingInvoice.Borrow.ReturnDate.CompareTo(todaysDate) > 90)
                {
                    borrowManager.RemoveInvoice(existingInvoice, existingInvoice.InvoiceSum);
                    borrowManager.CreateInvoice(customer, borrowOverdueReturnDate[i], (overdueCharge * 4), (totalDebt + (overdueCharge * 3)));
                }
            }
            return CreateInvoiceStatusCodes.Ok;
        }
    }
}
