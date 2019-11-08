using DataInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DataAccess
{
    public class CustomerManager : ICustomerManager
    {
        public Customer AddCustomer(int customerNumber, string customerName, string birthDate, string address, decimal debt, 
            Customer guardian, bool isGuardian)
        {
            using var libraryContext = new LibraryContext();
            var customer = new Customer();
            customer.CustomerNumber = customerNumber;
            customer.CustomerName = customerName;
            customer.BirthDate = birthDate;
            customer.Address = address;
            customer.Debt = debt;
            customer.Guardian = guardian;
            customer.IsGuardian = isGuardian;
            libraryContext.Customers.Add(customer);
            libraryContext.SaveChanges();
            return customer;
        }        

        public void SetCustomerAsGuardian(Customer customer, bool isGuardian)
        {
            using var libraryContext = new LibraryContext();            
            customer.IsGuardian = isGuardian;
            libraryContext.SaveChanges();
        }

        public void CancelCustomer(int customerID)
        {
            using var libraryContext = new LibraryContext();
            var customer = (from c in libraryContext.Customers
                            where c.CustomerID== customerID
                            select c).FirstOrDefault();
            libraryContext.Customers.Remove(customer);
            libraryContext.SaveChanges();
        }

        public Customer GetCustomerByCustomerNumber(int customerNumber)
        {
            using var libraryContext = new LibraryContext();
            return (from c in libraryContext.Customers
                    where c.CustomerNumber == customerNumber
                    select c).FirstOrDefault();
        }      
    }
}
