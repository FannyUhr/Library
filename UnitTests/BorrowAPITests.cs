using DataInterface;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class BorrowAPITests
    {
        
        //ADD BORROW:
        private AddBorrowStatusCodes AddBorrowNumberOne(Mock<IBorrowManager> borrowManagerMock)
        {            
            var customerManagerMock = SetupMock(new Customer{ CustomerNumber = 000001 });
            var bookManagerMock = SetupMock(new Book 
            { 
                BookTitle = "Circe",
                IsBorrowed = false
            });

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var successfull = borrowAPI.AddBorrow(000001, "Circe");
            return successfull;
        }

        [TestMethod]
        public void AddBorrowOk()
        {
            var borrowManagerMock = new Mock<IBorrowManager>();
            var customerManagerMock = SetupMock(new Customer { CustomerNumber = 000001 });
            var bookManagerMock = SetupMock(new Book 
            { 
                BookTitle = "Circe",
                IsBorrowed = false
            });

            borrowManagerMock.Setup(m =>
                m.GetAllCurrentBorrowFromCustomer(It.IsAny<int>()))
                .Returns(new List<Borrow>
                {
                    new Borrow
                    {
                        CustomerID = 1,
                        BookID = 1
                    },
                });

            var successfull = AddBorrowNumberOne(borrowManagerMock);
            Assert.AreEqual(AddBorrowStatusCodes.Ok, successfull);
            borrowManagerMock.Verify(
                m => m.AddBorrow(It.Is<int>(i => i == 000001), It.Is<string>(i => i == "Circe"),
                It.IsAny<DateTime>(), It.IsAny<DateTime>()),
                    Times.Once());

            borrowManagerMock.Verify(
                m => m.AddBorrowToBook(It.Is<string>(i => i == "Circe"), It.Is<bool>(i => i == true), It.IsAny<Borrow>()),
                    Times.Once());

            borrowManagerMock.Verify(
                m => m.AddToHistoryListOfBorrow(It.Is<int>(i => i == 000001), It.Is<string>(i => i == "Circe"), It.IsAny<DateTime>()),
                    Times.Once());
        }
        
        [TestMethod]
        public void AddBorrowNoSuchBook()
        {
            var borrowManagerMock = new Mock<IBorrowManager>();
            var customerManagerMock = SetupMock(new Customer { CustomerNumber = 000001 });
            var bookManagerMock = SetupMock((Book)null);
            
            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.AddBorrow(000001, "HejHopp");
            Assert.AreEqual(AddBorrowStatusCodes.NoSuchBook, result);
            borrowManagerMock.Verify(
                m => m.AddBorrow(000001, "HejHopp", new DateTime(), new DateTime()), Times.Never());            
        }

        [TestMethod]
        public void AddBorrowNoSuchCustomer()
        {
            var borrowManagerMock = new Mock<IBorrowManager>();
            var customerManagerMock = SetupMock((Customer)null);
            var bookManagerMock = SetupMock(new Book { BookTitle = "Circe" });

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.AddBorrow(000020, "Circe");
            Assert.AreEqual(AddBorrowStatusCodes.NoSuchCustomer, result);
            borrowManagerMock.Verify(
                m => m.AddBorrow(000020, "Circe", new DateTime(), new DateTime()), Times.Never());
        }

        [TestMethod]
        public void AddBorrowBookIsUnavailable()
        {
            var borrowManagerMock = new Mock<IBorrowManager>();
            var customerManagerMock = SetupMock(new Customer { CustomerNumber = 000001 });
            var bookManagerMock = SetupMock(new Book 
            { 
                BookTitle = "Flights",
                IsBorrowed = true
            });

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.AddBorrow(000001, "Flights");
            Assert.AreEqual(AddBorrowStatusCodes.BookIsUnavailable, result);
            borrowManagerMock.Verify(
                m => m.AddBorrow(000001, "Flights", new DateTime(), new DateTime()), Times.Never());
        }

        [TestMethod]
        public void AddBorrowCustomerHasDebt()
        {
            var borrowManagerMock = new Mock<IBorrowManager>();
            var customerManagerMock = SetupMock(new Customer 
            { 
                CustomerNumber = 000001,
                Debt = 100
            });
            var bookManagerMock = SetupMock(new Book
            {
                BookTitle = "Circe",
                IsBorrowed = false
            });
            borrowManagerMock.Setup(m =>
                m.GetAllCurrentBorrowFromCustomer(It.IsAny<int>()))
                .Returns(new List<Borrow>
                {
                    new Borrow
                    {
                        CustomerID = 1,
                        BookID = 1
                    },
                });

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.AddBorrow(000001, "Circe");            
            Assert.AreEqual(AddBorrowStatusCodes.CustomerHasDebt, result);
            borrowManagerMock.Verify(
                m => m.AddBorrow(It.Is<int>(i => i == 000001), It.Is<string>(i => i == "Circe"),
                It.IsAny<DateTime>(), It.IsAny<DateTime>()),
                    Times.Never());
        }

        [TestMethod]
        public void AddBorrowCustomerMaxBorrow()
        {
            var borrowManagerMock = new Mock<IBorrowManager>();
            var customerManagerMock = SetupMock(new Customer { CustomerID = 1 });
            var bookManagerMock = SetupMock(new Book
            {
                BookTitle = "Circe",
                IsBorrowed = false
            });

            borrowManagerMock.Setup(m =>
                m.GetAllCurrentBorrowFromCustomer(It.IsAny<int>()))
                .Returns(new List<Borrow>
                {
                    new Borrow
                    {
                        CustomerID = 1,
                        BookID = 1                        
                    },
                    new Borrow
                    {
                        CustomerID = 1,
                        BookID = 2
                    },
                    new Borrow
                    {
                        CustomerID = 1,
                        BookID = 3
                    },
                    new Borrow
                    {
                        CustomerID = 1,
                        BookID = 4
                    },
                    new Borrow
                    {
                        CustomerID = 1,
                        BookID = 5
                    },
                });

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.AddBorrow(000001, "Circe");
            Assert.AreEqual(AddBorrowStatusCodes.CustomerCanNotExceedMaxBookQuantity, result);
            borrowManagerMock.Verify(
                m => m.AddBorrow(It.Is<int>(i => i == 000001), It.Is<string>(i => i == "Circe"),
                It.IsAny<DateTime>(), It.IsAny<DateTime>()),
                    Times.Never());
        }

        [TestMethod]
        public void ExtendBorrowOk()
        {
            var customerManagerMock = SetupMock(new Customer
            {
                CustomerID = 1,
                Debt = 0
            });
            var bookManagerMock = SetupMock(new Book
            {
                BookID = 1,
                IsBorrowed = true
            });
            var borrowManagerMock = SetupMock(new Borrow
            {
                BorrowID = 1,
                CustomerID = 1,
                BookID = 1
            });

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.AddBorrow(000001, "Circe");
            Assert.AreEqual(AddBorrowStatusCodes.ExtendBorrowForCustomer, result);
            borrowManagerMock.Verify(
                m => m.ExtendBorrowDate(It.Is<int>(i => i == 1), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
                    Times.Once());

            borrowManagerMock.Verify(
                m => m.AddToHistoryListOfBorrow(It.Is<int>(i => i == 000001), It.Is<string>(i => i == "Circe"), It.IsAny<DateTime>()),
                    Times.Once());
        }

        [TestMethod]
        public void ExtendBorrowCustomerHasDebt()
        {
            var customerManagerMock = SetupMock(new Customer
            {
                CustomerID = 1,
                Debt = 100
            });
            var bookManagerMock = SetupMock(new Book
            {
                BookID = 1,
                IsBorrowed = true
            });
            var borrowManagerMock = SetupMock(new Borrow
            {
                BorrowID = 1,
                CustomerID = 1,
                BookID = 1
            });

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.AddBorrow(000001, "Circe");
            Assert.AreEqual(AddBorrowStatusCodes.CustomerHasDebtCanNotExtendBorrow, result);
            borrowManagerMock.Verify(
                m => m.ExtendBorrowDate(It.Is<int>(i => i == 1), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
                    Times.Never());
        }


        //RETURN BORROW:
        [TestMethod]
        public void ReturnBorrowOk()
        {
            var customerManagerMock = SetupMock(new Customer { CustomerNumber = 000001 });
            var bookManagerMock = SetupMock(new Book { BookTitle = "Circe" });
            var borrowManagerMock = SetupMock(new Borrow
            {
                BorrowID = 1
            });
            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.ReturnBorrow(000001, "Circe", 4);
            Assert.AreEqual(ReturnBorrowStatusCodes.Ok, result);
            borrowManagerMock.Verify(m =>
                m.ReturnBorrow(It.IsAny<int>(), It.IsAny<Book>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ReturnBorrowNoSuchCustomer()
        {
            var customerManagerMock = SetupMock((Customer)null);
            var bookManagerMock = SetupMock(new Book { BookTitle = "Circe" });
            var borrowManagerMock = SetupMock(new Borrow
            {
                BorrowID = 1
            });
            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.ReturnBorrow(000001, "Circe", 4);
            Assert.AreEqual(ReturnBorrowStatusCodes.NoSuchCustomer, result);
            borrowManagerMock.Verify(m =>
                m.ReturnBorrow(It.IsAny<int>(), It.IsAny<Book>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ReturnBorrowNoSuchBook()
        {
            var customerManagerMock = SetupMock(new Customer { CustomerNumber = 000001 });
            var bookManagerMock = SetupMock((Book)null);
            var borrowManagerMock = SetupMock(new Borrow
            {
                BorrowID = 1
            });
            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.ReturnBorrow(000001, "Circe", 4);
            Assert.AreEqual(ReturnBorrowStatusCodes.NoSuchBook, result);
            borrowManagerMock.Verify(m =>
                m.ReturnBorrow(It.IsAny<int>(), It.IsAny<Book>(), It.IsAny<int>()), Times.Never);
        }
        [TestMethod]
        public void ReturnBorrowNoSuchBorrow()
        {
            var customerManagerMock = SetupMock(new Customer { CustomerNumber = 000001 });
            var bookManagerMock = SetupMock(new Book { BookTitle = "Circe" });
            var borrowManagerMock = SetupMock((Borrow)null);
            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.ReturnBorrow(000001, "Circe", 4);
            Assert.AreEqual(ReturnBorrowStatusCodes.NoSuchBorrow, result);
            borrowManagerMock.Verify(m =>
                m.ReturnBorrow(It.IsAny<int>(), It.IsAny<Book>(), It.IsAny<int>()), Times.Never);
        }


        //CREATEINVOICE:
        [TestMethod]
        public void CreateInvoiceOk()
        {
            var bookManagerMock = new Mock<IBookManager>();
            var borrowManagerMock = new Mock<IBorrowManager>();
            var customerManagerMock = new Mock<ICustomerManager>();
            
            var todayDate = DateTime.Today;
            
            borrowManagerMock.Setup(m =>
                m.GetBorrowWithOverdueReturnDate(It.IsAny<DateTime>()))
                .Returns(new List<Borrow>
                {
                    new Borrow
                    {
                        BorrowID = 1,
                        ReturnDate = todayDate.AddDays(-20)
                    }
                });

           borrowManagerMock.Setup(m =>
                m.GetCustomerByBorrow(It.IsAny<Borrow>()))
                .Returns(new Customer { CustomerNumber = 000001 });

            borrowManagerMock.Setup(m =>
                m.GetInvoiceByBorrow(It.IsAny<Borrow>()))
                .Returns((Invoice)null);

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.CreateInvoice();
            Assert.AreEqual(CreateInvoiceStatusCodes.Ok, result);
            borrowManagerMock.Verify(m =>
                m.CreateInvoice(It.IsAny<Customer>(), It.IsAny<Borrow>(), It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Once);
        }

        [TestMethod]
        public void CreateInvoiceNoBorrowOverdueDate()
        {
            var bookManagerMock = new Mock<IBookManager>();
            var borrowManagerMock = new Mock<IBorrowManager>();
            var customerManagerMock = new Mock<ICustomerManager>();

            var todayDate = DateTime.Today;

            borrowManagerMock.Setup(m =>
                m.GetBorrowWithOverdueReturnDate(It.IsAny<DateTime>()))
                .Returns(new List<Borrow>());

            borrowManagerMock.Setup(m =>
                 m.GetCustomerByBorrow(It.IsAny<Borrow>()))
                 .Returns(new Customer { CustomerNumber = 000001 });

            borrowManagerMock.Setup(m =>
                m.GetInvoiceByBorrow(It.IsAny<Borrow>()))
                .Returns((Invoice)null);

            var borrowAPI = new BorrowAPI(borrowManagerMock.Object, customerManagerMock.Object, bookManagerMock.Object);
            var result = borrowAPI.CreateInvoice();
            Assert.AreEqual(CreateInvoiceStatusCodes.NoBorrowOverdueDate, result);
            borrowManagerMock.Verify(m =>
                m.CreateInvoice(It.IsAny<Customer>(), It.IsAny<Borrow>(), It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Never);
        }


        //SETUPMOCK:
        private static Mock<IBorrowManager> SetupMock(Borrow borrow)
        {
            var borrowManagerMock = new Mock<IBorrowManager>();
            borrowManagerMock.Setup(m =>
                    m.GetBorrowFromCustomerAndBook(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(borrow);

            borrowManagerMock.Setup(m =>
                m.AddBorrow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()));
            return borrowManagerMock;
        }

        private static Mock<ICustomerManager> SetupMock(Customer customer)
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            customerManagerMock.Setup(m =>
                    m.GetCustomerByCustomerNumber(It.IsAny<int>()))
                .Returns(customer);

            customerManagerMock.Setup(m =>
                m.AddCustomer(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<Customer>(), It.IsAny<bool>()));
            return customerManagerMock;
        }

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
    }
}
