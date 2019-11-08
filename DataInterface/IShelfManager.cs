using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public interface IShelfManager
    {
        Shelf AddShelf(int shelfNumber, int aisleNumber);
        Shelf GetShelfFromAisle(int shelfNumber, int aisleNumber);
        Shelf GetShelfByShelfId(int shelfID);
        void ChangeShelfNumber(int shelfID, int shelfNumber);
        void MoveShelf(int shelfID, int aisleID);
        void RemoveShelf(int shelfID);
    }
}
