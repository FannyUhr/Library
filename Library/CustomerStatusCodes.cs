using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public enum AddCustomerStatusCodes
    {
        CustomerAlreadyExist,
        InvalidBirthDate,
        CustomerIsMinor,
        Ok
    }

    public enum CancelCustomerStatusCodes
    {
        NoSuchCustomer,
        CustomerIsGuardian,
        CustomerHasUnpaidDebt,
        CustomerHasBorrowedBooks,
        Ok
    }
}
