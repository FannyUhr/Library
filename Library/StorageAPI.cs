using DataInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Library
{
    public class StorageAPI
    {
        private IAisleManager aisleManager;
        private IShelfManager shelfManager;

        public StorageAPI(IAisleManager aisleManager, IShelfManager shelfManager)
        {
            this.aisleManager = aisleManager;
            this.shelfManager = shelfManager;            
        }

        public bool AddAisle(int aisleNumber)
        {
            var newAisle = aisleManager.GetAisleByAisleNumber(aisleNumber);
            if (newAisle != null)
                return false;
            aisleManager.AddAisle(aisleNumber);
            return true;
        }

        public RemoveAisleErrorCodes RemoveAisle(int aisleNumber)
        {
            var newAisle = aisleManager.GetAisleByAisleNumber(aisleNumber);            
            if (newAisle == null)
                return RemoveAisleErrorCodes.NoSuchAisle;
            if (newAisle.Shelfs.Count > 0)
                return RemoveAisleErrorCodes.AisleHasShelfs;

            aisleManager.RemoveAisle(newAisle.AisleID);
            return RemoveAisleErrorCodes.Ok;
        }

        public AddShelfStatusCodes AddShelf(int shelfNumber, int aisleNumber)
        {
            var newAisle = aisleManager.GetAisleByAisleNumber(aisleNumber);
            if (newAisle == null)
                return AddShelfStatusCodes.NoSuchAisle;
            var existingShelf = shelfManager.GetShelfFromAisle(shelfNumber, aisleNumber);
            if (existingShelf != null)
                return AddShelfStatusCodes.ShelfAlreadyExist;
            
            shelfManager.AddShelf(shelfNumber, aisleNumber);
            return AddShelfStatusCodes.Ok;            
        }

        public ChangeShelfNumberStatusCodes ChangeShelfNumber(int shelfID, int shelfNumber)
        {
            var currentShelf = shelfManager.GetShelfByShelfId(shelfID);
            if (currentShelf == null)
                return ChangeShelfNumberStatusCodes.NoSuchShelf;
            if (currentShelf.ShelfNumber == shelfNumber)
                return ChangeShelfNumberStatusCodes.CanNotChangeToSameNumber;
            shelfManager.ChangeShelfNumber(shelfID, shelfNumber);
            return ChangeShelfNumberStatusCodes.Ok;
        }

        public MoveShelfStatusCodes MoveShelf(int shelfNumber, int aisleNumber)
        {
            var newAisle = aisleManager.GetAisleByAisleNumber(aisleNumber);
            if (newAisle == null)
                return MoveShelfStatusCodes.NoSuchAisle;
            var shelf = shelfManager.GetShelfFromAisle(shelfNumber, aisleNumber);
            if (shelf == null)
                return MoveShelfStatusCodes.NoSuchShelf;            
            if (shelf.Aisle.AisleNumber == aisleNumber)
                return MoveShelfStatusCodes.ShelfAlreadyInThatAisle;
            
            shelfManager.MoveShelf(shelf.ShelfID, newAisle.AisleID);
            return MoveShelfStatusCodes.Ok;
        }

        public RemoveShelfStatusCodes RemoveShelf(int shelfNumber, int aisleNumber)
        {
            var newShelf = shelfManager.GetShelfFromAisle(shelfNumber, aisleNumber);
            if (newShelf == null)
                return RemoveShelfStatusCodes.NoSuchShelf;
            if (newShelf.Books.Count > 0)
                return RemoveShelfStatusCodes.ShelfHasBooks;

            shelfManager.RemoveShelf(newShelf.ShelfID);
            return RemoveShelfStatusCodes.Ok;
        }
    }
}
