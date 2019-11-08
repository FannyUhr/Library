using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public enum RemoveAisleErrorCodes
    {
        NoSuchAisle,
        AisleHasShelfs,
        Ok
    }

    public enum AddShelfStatusCodes
    {
        NoSuchAisle,
        ShelfAlreadyExist,
        Ok
    }

    public enum ChangeShelfNumberStatusCodes
    {
        NoSuchShelf,
        CanNotChangeToSameNumber,
        Ok
    }

    public enum MoveShelfStatusCodes
    {
        NoSuchAisle,
        NoSuchShelf,    
        ShelfAlreadyInThatAisle,
        Ok
    }

    public enum RemoveShelfStatusCodes
    {        
        NoSuchShelf,
        ShelfHasBooks,
        Ok
    }
}
