using DataInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;

namespace Library
{
    public class CustomerAPI
    {
        private ICustomerManager customerManager;
        private IBorrowManager borrowManager;

        public CustomerAPI(ICustomerManager customerManager, IBorrowManager borrowManager)
        {
            this.customerManager = customerManager;
            this.borrowManager = borrowManager;
        }

        public bool ValidateBirthDate(string birthDate)
        {
            DateTime temp;
            if ((DateTime.TryParseExact(birthDate, "yyyy-mm-dd",
                new CultureInfo("sv-SE"),
                DateTimeStyles.None,
                out temp)) && temp < DateTime.Now)
                return true;
            return false;
        }

        public int GetAge(string birthDate)
        {
            DateTime temp;
            DateTime.TryParse(birthDate, out temp);

            int now = int.Parse(DateTime.Now.ToString("yyyymmdd"));
            int dateOfBirth = int.Parse(temp.ToString("yyyymmdd"));            
            int age = (now - dateOfBirth) / 10000;
            return age;
        }

        public AddCustomerStatusCodes AddCustomer(int customerNumber, string customerName, string birthDate, string address, Customer guardian)
        {            
            decimal newDebt = 0;
            var existingCustomer = customerManager.GetCustomerByCustomerNumber(customerNumber);
            if (existingCustomer != null)
                return AddCustomerStatusCodes.CustomerAlreadyExist;
            if (ValidateBirthDate(birthDate) == false)
                return AddCustomerStatusCodes.InvalidBirthDate;
            if (GetAge(birthDate) < 15)
            {
                customerManager.AddCustomer(customerNumber, customerName, birthDate, address, newDebt, guardian, false);
                customerManager.SetCustomerAsGuardian(guardian, true);
                return AddCustomerStatusCodes.CustomerIsMinor;
            }
                       
            customerManager.AddCustomer(customerNumber, customerName, birthDate, address, newDebt, null, false);
            return AddCustomerStatusCodes.Ok;                         
        }

        public CancelCustomerStatusCodes CancelCustomer(int customerNumber)
        {
            var customer = customerManager.GetCustomerByCustomerNumber(customerNumber);
            if (customer == null)
                return CancelCustomerStatusCodes.NoSuchCustomer;
            if (customer.IsGuardian == true)
                return CancelCustomerStatusCodes.CustomerIsGuardian;
            if (customer.Debt > 0)
                return CancelCustomerStatusCodes.CustomerHasUnpaidDebt;
            var borrow = borrowManager.GetAllCurrentBorrowFromCustomer(customerNumber);
            if (borrow.Count > 0)
                return CancelCustomerStatusCodes.CustomerHasBorrowedBooks;

            customerManager.CancelCustomer(customer.CustomerID);
            return CancelCustomerStatusCodes.Ok;
        }
    }
}
