using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public interface ICustomerManager
    {
        Customer AddCustomer(int customerNumber, string customerName, string birthDate, string address, decimal debt,
            Customer guardian, bool isGuardian);
        void SetCustomerAsGuardian(Customer customer, bool isGuardian);
        void CancelCustomer(int customerID);
        Customer GetCustomerByCustomerNumber(int customerNumber);
    }
}
