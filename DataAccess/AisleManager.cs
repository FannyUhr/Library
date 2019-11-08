using DataInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AisleManager : IAisleManager
    {
        public void AddAisle(int aisleNumber)
        {
            using var libraryContext = new LibraryContext();            
            var aisle = new Aisle();
            aisle.AisleNumber = aisleNumber;
            libraryContext.Aisles.Add(aisle);
            libraryContext.SaveChanges();
        }

        public void RemoveAisle(int aisleID)
        {
            using var libraryContext = new LibraryContext();
            var aisle = (from a in libraryContext.Aisles
                         where a.AisleID == aisleID
                         select a).FirstOrDefault();
            libraryContext.Aisles.Remove(aisle);
            libraryContext.SaveChanges();
        }

        public Aisle GetAisleByAisleNumber(int aisleNumber)
        {
            using var libraryContext = new LibraryContext();
            return (from a in libraryContext.Aisles
                    where a.AisleNumber == aisleNumber
                    select a)
                    .Include(a => a.Shelfs)
                    .FirstOrDefault();
        }                 
    }
}
