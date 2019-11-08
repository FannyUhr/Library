using DataInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ShelfManager : IShelfManager
    {
        public Shelf AddShelf(int shelfNumber, int aisleNumber)
        {
            using var libraryContext = new LibraryContext();
            
            var aisle = (from a in libraryContext.Aisles
                         where a.AisleNumber == aisleNumber
                         select a).FirstOrDefault();

            var shelf = new Shelf();
            shelf.ShelfNumber = shelfNumber;
            shelf.AisleID = aisle.AisleID;
            libraryContext.Shelfs.Add(shelf);
            libraryContext.SaveChanges();
            return shelf;
        }

        public void ChangeShelfNumber(int shelfID, int shelfNumber)
        {
            using var libraryContext = new LibraryContext();
            var shelf = (from s in libraryContext.Shelfs
                         where s.ShelfID == shelfID
                         select s)
                         .FirstOrDefault();
            shelf.ShelfNumber = shelfNumber;
            libraryContext.SaveChanges();
        }

        public void MoveShelf(int shelfID, int aisleID)
        {
            using var libraryContext = new LibraryContext();
            var shelf = (from s in libraryContext.Shelfs
                         where s.ShelfID == shelfID
                         select s)
                         .FirstOrDefault();
            shelf.AisleID = aisleID;
            libraryContext.SaveChanges();
        }

        public void RemoveShelf(int shelfID)
        {
            using var libraryContext = new LibraryContext();
            var shelf = (from s in libraryContext.Shelfs
                         where s.ShelfID == shelfID
                         select s).FirstOrDefault();
            libraryContext.Shelfs.Remove(shelf);
            libraryContext.SaveChanges();
        }

        public Shelf GetShelfFromAisle(int shelfNumber, int aisleNumber)
        {
            using var libraryContext = new LibraryContext();
            return (from s in libraryContext.Shelfs
                 join a in libraryContext.Aisles
                 on s.AisleID equals a.AisleID
                 where s.ShelfNumber == shelfNumber && a.AisleNumber == aisleNumber
                 select s).FirstOrDefault();
        }

        public Shelf GetShelfByShelfId(int shelfID)
        {
            using var libraryContext = new LibraryContext();
            return (from s in libraryContext.Shelfs
                    where s.ShelfID == shelfID
                    select s).FirstOrDefault();
        }        
    }
}
