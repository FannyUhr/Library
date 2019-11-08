using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public enum AddBookStatusCodes
    {
        NoSuchShelf,
        Ok
    }

    public enum MoveBookStatusCodes
    {
        NoSuchBook,
        NoSuchShelf,
        BookAlreadyInThatShelf,
        Ok
    }

    public enum RemoveBookStatusCodes
    {
        NoSuchBook,
        BookIsBorrowed,
        Ok
    }

    public enum CreateDiscardListStatusCodes
    {
        NoBooksInConditionOne,
        Ok
    }

    public enum ClearDiscardListStatusCodes
    {
        NoDiscardBooksToClear,
        Ok
    }
}
