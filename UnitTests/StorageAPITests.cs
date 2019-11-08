using DataInterface;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class StorageAPITests
    {
        
        //TEST ADD AISLE:
        private static bool AddAisleNumberOne(Mock<IAisleManager> aisleManagerMock)
        {
            var storageAPI = new StorageAPI(aisleManagerMock.Object, null);
            var successfull = storageAPI.AddAisle(1);
            return successfull;
        }

        [TestMethod]
        public void TestAddAisle()
        {
            Mock<IAisleManager> aisleManagerMock = SetupMock((Aisle)null);
            bool successfull = AddAisleNumberOne(aisleManagerMock);
            Assert.IsTrue(successfull);
            aisleManagerMock.Verify(
                m => m.AddAisle(It.Is<int>(i => i == 1)),
                    Times.Once());
        }       

        [TestMethod]
        public void TestAddExistingAisle()
        {
            var aisleManagerMock = SetupMock(new Aisle());
            bool successfull = AddAisleNumberOne(aisleManagerMock);
            Assert.IsFalse(successfull);
            aisleManagerMock.Verify(
                m => m.AddAisle(It.Is<int>(i => i == 1)),
                    Times.Never());
        }


        //REMOVE AISLE:
        [TestMethod]
        public void RemoveEmptyAisle()
        {
            var aisleManagerMock = SetupMock(new Aisle 
                { 
                    AisleNumber = 4,
                    Shelfs = new List<Shelf>()
                });
            var shelfManagerMock = new Mock<IShelfManager>();

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var successfull = storageAPI.RemoveAisle(4);
            Assert.AreEqual(RemoveAisleErrorCodes.Ok, successfull);
            aisleManagerMock.Verify(m =>
                m.RemoveAisle(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void RemoveAisleWithOneShelf()
        {            
            var aisleManagerMock = SetupMock(new Aisle
                {
                    AisleNumber = 4,
                    Shelfs = new List<Shelf>
                    {
                        new Shelf()
                    }
                });
            var shelfManagerMock = new Mock<IShelfManager>();

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var successfull = storageAPI.RemoveAisle(4);
            Assert.AreEqual(RemoveAisleErrorCodes.AisleHasShelfs, successfull);
            aisleManagerMock.Verify(m =>
               m.RemoveAisle(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void RemoveNonExistingAisle()
        {
            var aisleManagerMock = SetupMock((Aisle)null);
            var shelfManagerMock = new Mock<IShelfManager>();

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var successfull = storageAPI.RemoveAisle(4);
            Assert.AreEqual(RemoveAisleErrorCodes.NoSuchAisle, successfull);
            aisleManagerMock.Verify(m =>
               m.RemoveAisle(It.IsAny<int>()), Times.Never);
        }



        //TEST ADD SHELF:
        private AddShelfStatusCodes AddShelfNumberOne(Mock<IShelfManager> shelfManagerMock)
        {
            var aisleManagerMock = SetupMock(new Aisle { AisleNumber = 1 });
            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var successfull = storageAPI.AddShelf(1, 1);
            return successfull;
        }

        [TestMethod]
        public void TestAddShelfOk()
        {
            var shelfManagerMock = new Mock<IShelfManager>();
            var aisleManagerMock = SetupMock(new Aisle { AisleNumber = 1 });

            var successfull = AddShelfNumberOne(shelfManagerMock);
            Assert.AreEqual(AddShelfStatusCodes.Ok, successfull);
            shelfManagerMock.Verify(
                m => m.AddShelf(It.Is<int>(i => i == 1), It.Is<int>(i => i == 1)),
                    Times.Once());
        }

        [TestMethod]
        public void TestAddExistingShelf()
        {
            var aisleManagerMock = SetupMock(new Aisle { AisleNumber = 1 });
            var shelfManagerMock = SetupMock(new Shelf());            

            var successfull = AddShelfNumberOne(shelfManagerMock);
            Assert.AreEqual(AddShelfStatusCodes.ShelfAlreadyExist, successfull);
            shelfManagerMock.Verify(
                m => m.AddShelf(It.Is<int>(i => i == 1), It.Is<int>(i => i == 1)),
                    Times.Never());
        }

        [TestMethod]
        public void AddShelfToNonExistingAisle()
        {                        
            var aisleManagerMock = SetupMock((Aisle)null);
            var shelfManagerMock = new Mock<IShelfManager>();

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var result = storageAPI.AddShelf(1, 1);
            Assert.AreEqual(AddShelfStatusCodes.NoSuchAisle, result);
            shelfManagerMock.Verify(
                m => m.AddShelf(1, 1), Times.Never());
        }


        //CHANGE SHELFNUMBER:
        [TestMethod]
        public void ChangeShelfNumberOk()
        {
            var aisleManagerMock = SetupMock(new Aisle { AisleID = 1 });
            var shelfManagerMock = new Mock<IShelfManager>();

            shelfManagerMock.Setup(m =>
                    m.GetShelfByShelfId(It.IsAny<int>()))
                .Returns(new Shelf
                {
                    ShelfID = 1,
                    Aisle = new Aisle(),
                    ShelfNumber = 1
                });

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var result = storageAPI.ChangeShelfNumber(1, 2);
            Assert.AreEqual(ChangeShelfNumberStatusCodes.Ok, result);
            shelfManagerMock.Verify(m =>
                m.ChangeShelfNumber(1, 2), Times.Once());
        }

        [TestMethod]
        public void ChangeShelfNumberNoSuchShelf()
        {
            var shelfManagerMock = SetupMock((Shelf)null);

            var storageAPI = new StorageAPI(null, shelfManagerMock.Object);
            var result = storageAPI.ChangeShelfNumber(1, 2);
            Assert.AreEqual(ChangeShelfNumberStatusCodes.NoSuchShelf, result);
            shelfManagerMock.Verify(m =>
                m.ChangeShelfNumber(1, 2), Times.Never());
        }

        [TestMethod]
        public void ChangeShelfNumberToSameShelfNumber()
        {
            var aisleManagerMock = SetupMock(new Aisle { AisleID = 1 });
            var shelfManagerMock = new Mock<IShelfManager>();

            shelfManagerMock.Setup(m =>
                    m.GetShelfByShelfId(It.IsAny<int>()))
                .Returns(new Shelf
                {
                    ShelfID = 1,
                    Aisle = new Aisle(),
                    ShelfNumber = 1
                });

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var result = storageAPI.ChangeShelfNumber(1, 1);
            Assert.AreEqual(ChangeShelfNumberStatusCodes.CanNotChangeToSameNumber, result);
            shelfManagerMock.Verify(m =>
                m.ChangeShelfNumber(1, 1), Times.Never());
        }


        //MOVESHELF:
        [TestMethod]
        public void MoveShelfOk()
        {
            var aisleManagerMock = SetupMock(new Aisle { AisleID = 2 });
            var shelfManagerMock = SetupMock(new Shelf
               {
                   ShelfID = 2,
                   Aisle = new Aisle()
               });

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var result = storageAPI.MoveShelf(1, 1);
            Assert.AreEqual(MoveShelfStatusCodes.Ok, result);
            shelfManagerMock.Verify(m =>
                m.MoveShelf(2, 2), Times.Once());
        }

        [TestMethod]
        public void MoveShelfNoSuchAisle()
        {
            var aisleManagerMock = SetupMock((Aisle)null);
            var shelfManagerMock = new Mock<IShelfManager>();

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var result = storageAPI.MoveShelf(1, 1);
            Assert.AreEqual(MoveShelfStatusCodes.NoSuchAisle, result);
            shelfManagerMock.Verify(m =>
                m.MoveShelf(2, 2), Times.Never());
        }

        [TestMethod]
        public void MoveShelfNoSuchShelf()
        {
            var aisleManagerMock = SetupMock(new Aisle { AisleID = 2 });
            var shelfManagerMock = SetupMock((Shelf)null);

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var result = storageAPI.MoveShelf(1, 1);
            Assert.AreEqual(MoveShelfStatusCodes.NoSuchShelf, result);
            shelfManagerMock.Verify(m =>
                m.MoveShelf(2, 2), Times.Never());
        }

        [TestMethod]
        public void MoveShelfAlreadyInThatAisle()
        {
            var currentAisle = new Aisle { AisleNumber = 2 };
            var aisleManagerMock = SetupMock(currentAisle);
            var shelfManagerMock = SetupMock(new Shelf
               {
                   ShelfID = 2,
                   Aisle = currentAisle
               });

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var result = storageAPI.MoveShelf(2, 2);
            Assert.AreEqual(MoveShelfStatusCodes.ShelfAlreadyInThatAisle, result);
            shelfManagerMock.Verify(m =>
                m.MoveShelf(2, 2), Times.Never());
        }

        [TestMethod]
        public void RemoveEmptyShelf()
        {
            var aisleManagerMock = new Mock<IAisleManager>();
            var shelfManagerMock = SetupMock(new Shelf
                {
                    ShelfNumber = 1,
                    Books = new List<Book>()
                });

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var successfull = storageAPI.RemoveShelf(1, 2);
            Assert.AreEqual(RemoveShelfStatusCodes.Ok, successfull);
            shelfManagerMock.Verify(m =>
                m.RemoveShelf(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void RemoveShelfWithOneBook()
        {
            var aisleManagerMock = new Mock<IAisleManager>();
            var shelfManagerMock = SetupMock(new Shelf
                {
                    ShelfNumber = 4,                    
                    Books = new List<Book>
                    {
                        new Book()
                    }
                });

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var successfull = storageAPI.RemoveShelf(4, 1);
            Assert.AreEqual(RemoveShelfStatusCodes.ShelfHasBooks, successfull);
            shelfManagerMock.Verify(m =>
               m.RemoveShelf(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void RemoveNonExistingShelf()
        {
            var aisleManagerMock = new Mock<IAisleManager>();
            var shelfManagerMock = SetupMock((Shelf)null);

            var storageAPI = new StorageAPI(aisleManagerMock.Object, shelfManagerMock.Object);
            var successfull = storageAPI.RemoveShelf(4, 1);
            Assert.AreEqual(RemoveShelfStatusCodes.NoSuchShelf, successfull);
            shelfManagerMock.Verify(m =>
               m.RemoveShelf(It.IsAny<int>()), Times.Never);
        }

        private static Mock<IAisleManager> SetupMock(Aisle aisle)
        {
            var aisleManagerMock = new Mock<IAisleManager>();

            aisleManagerMock.Setup(m =>
                    m.GetAisleByAisleNumber(It.IsAny<int>()))
                .Returns(aisle);

            aisleManagerMock.Setup(m =>
                m.AddAisle(It.IsAny<int>()));
            return aisleManagerMock;
        }

        private static Mock<IShelfManager> SetupMock(Shelf shelf)
        {
            var shelfManagerMock = new Mock<IShelfManager>();

            shelfManagerMock.Setup(m =>
                    m.GetShelfFromAisle(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(shelf);                        

            shelfManagerMock.Setup(m =>
                m.AddShelf(It.IsAny<int>(), It.IsAny<int>()));
            return shelfManagerMock;
        }
    }
}
