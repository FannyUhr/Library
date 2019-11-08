using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public enum AddBorrowStatusCodes
    {
        NoSuchBook,
        NoSuchCustomer,
        BookIsUnavailable,
        CustomerCanNotExceedMaxBookQuantity, 
        ExtendBorrowForCustomer,
        CustomerHasDebtCanNotExtendBorrow, 
        CustomerHasDebt,
        Ok
    }

    public enum ReturnBorrowStatusCodes
    {
        NoSuchBook,
        NoSuchCustomer,
        NoSuchBorrow,
        Ok,
    }

    public enum CreateInvoiceStatusCodes
    {
        NoBorrowOverdueDate,
        Ok
    }
}
