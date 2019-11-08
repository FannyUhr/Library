using DataInterface;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class BookAPITests
    {
        //ADD BOOK:
        private AddBookStatusCodes AddFirstBookCirce(Mock<IBookManager> bookManagerMock)
        {                        
            var aisle = new Aisle { AisleNumber = 3 };
            var shelfManagerMock = SetupMock(new Shelf
                {
                    ShelfNumber = 1,
                    Aisle = aisle                
                });          

            var bookAPI = new BookAPI(shelfManagerMock.Object, bookManagerMock.Object);
            var successfull = bookAPI.AddBook("9781526610140", "Circe", "Madeline Miller", 199, 4, 1, 3);
            return successfull;
        }

        [TestMethod]
        public void TestAddBookOk()
        {
            var bookManagerMock = new Mock<IBookManager>();
            var aisle = new Aisle { AisleNumber = 3 };
            var shelfManagerMock = SetupMock(new Shelf
            {
                ShelfNumber = 1,
                Aisle = aisle
            });

            var successfull = AddFirstBookCirce(bookManagerMock);
            Assert.AreEqual(AddBookStatusCodes.Ok, successfull);
            bookManagerMock.Verify(
                m => m.AddBook(It.Is<string>(i => i == "9781526610140"), It.Is<string>(i => i == "Circe"), It.Is<string>(i => i == "Madeline Miller"), 
                It.Is<int>(i => i == 2019), It.Is<int>(i => i == 199), It.Is<int>(i => i == 4), It.IsAny<Shelf>(),
                It.Is<bool>(i => i == false), It.Is<Borrow>(i => i == null)),
                    Times.Once());
        }

        [TestMethod]
        public void AddBookToNonExistingShelf()
        {
            var shelfManagerMock = SetupMock((Shelf)null);
            var bookManagerMock = new Mock<IBookManager>();            

            var bookAPI = new BookAPI(shelfManagerMock.Object, bookManagerMock.Object);
            var result = bookAPI.AddBook("9781526610140", "Circe", "Madeline Miller", 199, 4, 1, 3);
            Assert.AreEqual(AddBookStatusCodes.NoSuchShelf, result);
            bookManagerMock.Verify(
                m => m.AddBook("9781526610140", "Circe", "Madeline Miller", 2019, 199, 4, new Shelf(), false, null), Times.Never());
        }        

        //MOVE BOOK:
        [TestMethod]
        public void MoveBookOk()
        {
            var shelfManagerMock = SetupMock(new Shelf { ShelfID = 2 });
            var bookManagerMock = SetupMock(new Book
               {
                   BookID = 2,
                   Shelf = new Shelf()
               });

            var bookAPI = new BookAPI(shelfManagerMock.Object, bookManagerMock.Object);
            var result = bookAPI.MoveBook("Circe", 1, 1);
            Assert.AreEqual(MoveBookStatusCodes.Ok, result);
            bookManagerMock.Verify(m =>
                m.MoveBook(2, 2), Times.Once());
        }

        [TestMethod]
        public void MoveBookNoSuchBook()
        {
            var shelfManagerMock = SetupMock(new Shelf { ShelfID = 2 });
            var bookManagerMock = SetupMock((Book)null);

            var bookAPI = new BookAPI(shelfManagerMock.Object, bookManagerMock.Object);
            var result = bookAPI.MoveBook("Circe", 1, 1);
            Assert.AreEqual(MoveBookStatusCodes.NoSuchBook, result);
            shelfManagerMock.Verify(m =>
                m.MoveShelf(2, 2), Times.Never());
        }

        [TestMethod]
        public void MoveBookNoSuchShelf()
        {
            var shelfManagerMock = SetupMock((Shelf)null);
            var bookManagerMock = SetupMock(new Book { BookID = 2 });

            var bookAPI = new BookAPI(shelfManagerMock.Object, bookManagerMock.Object);
            var result = bookAPI.MoveBook("Circe", 1, 1);
            Assert.AreEqual(MoveBookStatusCodes.NoSuchShelf, result);
            shelfManagerMock.Verify(m =>
                m.MoveShelf(2, 2), Times.Never());
        }

        [TestMethod]
        public void MoveBookAlreadyInThatShelf()
        {
            var currentAisle = new Aisle { AisleNumber = 1 };
            var currentShelf = (new Shelf 
            { 
                ShelfNumber = 1,
                Aisle = currentAisle
            });

            var shelfManagerMock = SetupMock(currentShelf);

            var bookManagerMock = SetupMock(new Book
               {
                   BookID = 1,
                   Shelf = currentShelf
               });

            var bookAPI = new BookAPI(shelfManagerMock.Object, bookManagerMock.Object);
            var result = bookAPI.MoveBook("Circe", 1, 1);
            Assert.AreEqual(MoveBookStatusCodes.BookAlreadyInThatShelf, result);
            bookManagerMock.Verify(m =>
                m.MoveBook(1, 1), Times.Never());
        }

        //REMOVE BOOK:
        [TestMethod]
        public void RemoveAvailableBook()
        {
            var bookManagerMock = SetupMock(new Book
            {
                BookID = 1,
                BookTitle = "Circe",
                IsBorrowed = false
            });

            var bookAPI = new BookAPI(null, bookManagerMock.Object);
            var successfull = bookAPI.RemoveBook("Circe");
            Assert.AreEqual(RemoveBookStatusCodes.Ok, successfull);
            bookManagerMock.Verify(m =>
                m.RemoveBook(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void RemoveBorrowedBook()
        {            
            var bookManagerMock = SetupMock(new Book
                {
                    BookID = 4,
                    BookTitle = "Circe",
                    IsBorrowed = true
                });

            var bookAPI = new BookAPI(null, bookManagerMock.Object);
            var successfull = bookAPI.RemoveBook("Circe");
            Assert.AreEqual(RemoveBookStatusCodes.BookIsBorrowed, successfull);
            bookManagerMock.Verify(m =>
               m.RemoveBook(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void RemoveNonExistingBook()
        {            
            var bookManagerMock = SetupMock((Book)null);

            var bookAPI = new BookAPI(null, bookManagerMock.Object);
            var successfull = bookAPI.RemoveBook("Circe");
            Assert.AreEqual(RemoveBookStatusCodes.NoSuchBook, successfull);
            bookManagerMock.Verify(m =>
               m.RemoveBook(It.IsAny<int>()), Times.Never);
        }

        //CREATE DISCARDLIST:
        [TestMethod]
        public void CreateDiscardListOk()
        {
            var bookManagerMock = new Mock<IBookManager>();
            bookManagerMock.Setup(m =>
                m.GetAllBooksWithConditionOne(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Book>
                {
                    new Book{ BookID = 1 }
                });

            var shelfManagerMock = SetupMock(new Shelf { ShelfID = 2 });

            var bookAPI = new BookAPI(shelfManagerMock.Object, bookManagerMock.Object);
            var result = bookAPI.CreateDiscardList();
            Assert.AreEqual(CreateDiscardListStatusCodes.Ok, result);
            bookManagerMock.Verify(
                m => m.CreateDiscardList(It.IsAny<Shelf>(), It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        public void CreateDiscardListNoBooksInBadCondition()
        {
            var bookManagerMock = new Mock<IBookManager>();
            bookManagerMock.Setup(m =>
                m.GetAllBooksWithConditionOne(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<Book>());

            var shelfManagerMock = SetupMock(new Shelf { ShelfID = 2 });

            var bookAPI = new BookAPI(shelfManagerMock.Object, bookManagerMock.Object);
            var result = bookAPI.CreateDiscardList();
            Assert.AreEqual(CreateDiscardListStatusCodes.NoBooksInConditionOne, result);
            bookManagerMock.Verify(
                m => m.CreateDiscardList(It.IsAny<Shelf>(), It.IsAny<Book>()), Times.Never);
        }

        //CLEAR DISCARDLIST:
        [TestMethod]
        public void ClearDiscardListOk()
        {
            var bookManagerMock = new Mock<IBookManager>();
            bookManagerMock.Setup(m =>
                m.GetAllBooksInDiscardList())
                .Returns(new List<Book>
                {
                    new Book(),
                    new Book() 
                });

            var bookAPI = new BookAPI(null, bookManagerMock.Object);
            var result = bookAPI.ClearDiscardList();
            Assert.AreEqual(ClearDiscardListStatusCodes.Ok, result);
            bookManagerMock.Verify(
                m => m.ClearDiscardList(It.IsAny<List<Book>>()), Times.Once);
        }
        [TestMethod]
        public void ClearEmptyDiscardList()
        {
            var bookManagerMock = new Mock<IBookManager>();
            bookManagerMock.Setup(m =>
                m.GetAllBooksInDiscardList())
                .Returns(new List<Book>());

            var bookAPI = new BookAPI(null, bookManagerMock.Object);
            var result = bookAPI.ClearDiscardList();
            Assert.AreEqual(ClearDiscardListStatusCodes.NoDiscardBooksToClear, result);
            bookManagerMock.Verify(
                m => m.ClearDiscardList(It.IsAny<List<Book>>()), Times.Never);
        }

        //SETUPMOCKS:
        private static Mock<IBookManager> SetupMock(Book book)
        {
            var bookManagerMock = new Mock<IBookManager>();
            bookManagerMock.Setup(m =>
                    m.GetBookByBookTitle(It.IsAny<string>()))
                .Returns(book);

            bookManagerMock.Setup(m =>
                m.AddBook(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Shelf>(), It.IsAny<bool>(), It.IsAny<Borrow>()));
            return bookManagerMock;
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
