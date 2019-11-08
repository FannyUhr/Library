using DataInterface;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class CustomerAPITests
    {        
        //VALIDATE BIRTHDATE:        
        [TestMethod]
        public void TestValidateBirthDateOk()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var customerApi = new CustomerAPI(customerManagerMock.Object, null);
            var result = customerApi.ValidateBirthDate("1996-07-22");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateBirthDateFailDigits()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var customerApi = new CustomerAPI(customerManagerMock.Object, null);
            var result = customerApi.ValidateBirthDate("12345");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateBirthDateFailLetters()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var customerApi = new CustomerAPI(customerManagerMock.Object, null);
            var result = customerApi.ValidateBirthDate("Hejhopp");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateBirthDateFailFormat()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var customerApi = new CustomerAPI(customerManagerMock.Object, null);
            var result = customerApi.ValidateBirthDate("1996/07/22");
            Assert.IsFalse(result);
        }

        //GET AGE:
        [TestMethod]
        public void TestGetAgeOk()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var customerApi = new CustomerAPI(customerManagerMock.Object, null);
            var result = customerApi.GetAge("1996-07-22");
            Assert.AreEqual(23, result);
        }

        [TestMethod]
        public void TestGetAgeFail()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var customerApi = new CustomerAPI(customerManagerMock.Object, null);
            var result = customerApi.GetAge("1996-07-22");
            Assert.AreNotEqual(32, result);
        }


        //ADD CUSTOMER:
        private AddCustomerStatusCodes AddCustomerNumberOne(Mock<ICustomerManager> customerManagerMock)
        {
            var customerAPI = new CustomerAPI(customerManagerMock.Object, null);
            var successfull = customerAPI.AddCustomer(000001, "Fanny Uhr", "1996-07-22", "Peppargatan 13", null);
            return successfull;
        }

        [TestMethod]
        public void TestAddCustomerOk()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var successfull = AddCustomerNumberOne(customerManagerMock);
            Assert.AreEqual(AddCustomerStatusCodes.Ok, successfull);
            customerManagerMock.Verify(
                m => m.AddCustomer(It.Is<int>(i => i == 000001), It.Is<string>(i => i == "Fanny Uhr"), It.Is<string>(i => i == "1996-07-22"),
                It.Is<string>(i => i == "Peppargatan 13"), It.Is<decimal>(i => i == 0), It.Is<Customer>(i => i == null), It.Is<bool>(i => i == false)),
                    Times.Once());
        }

        [TestMethod]
        public void AddExistingCustomer()
        {
            var customerManagerMock = SetupMock(new Customer());
            var successfull = AddCustomerNumberOne(customerManagerMock);
            Assert.AreEqual(AddCustomerStatusCodes.CustomerAlreadyExist, successfull);
            customerManagerMock.Verify(
                m => m.AddCustomer(It.Is<int>(i => i == 000001), It.Is<string>(i => i == "Fanny Uhr"), It.Is<string>(i => i == "1996-07-22"),
                It.Is<string>(i => i == "Peppargatan 13"), It.Is<decimal>(i => i == 0), It.Is<Customer>(i => i == null), It.Is<bool>(i => i == false)),
                    Times.Never());
        }

        [TestMethod]
        public void AddCustomerNoSuchBirthDate()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var customerApi = new CustomerAPI(customerManagerMock.Object, null);
            var result = customerApi.AddCustomer(000002, "Harald Hansson", "12345", "Wilhelmgatan 3", null);
            Assert.AreEqual(AddCustomerStatusCodes.InvalidBirthDate, result);
        }
        
        private AddCustomerStatusCodes AddMinorCustomerNumberOne(Mock<ICustomerManager> customerManagerMock)
        {
            var guardian = (new Customer
            {
                CustomerID = 1,
                BirthDate = "1973-07-14"
            });
            var customerAPI = new CustomerAPI(customerManagerMock.Object, null);
            var successfull = customerAPI.AddCustomer(000008, "Pontus Andersson", "2008-02-16", "Linfrögatan 22", guardian);
            return successfull;
        }

        [TestMethod]
        public void TestAddMinorCustomer()
        {
            var customerManagerMock = new Mock<ICustomerManager>();
            var guardian = (new Customer 
            { 
                CustomerID = 1,
                BirthDate = "1973-07-14"
            });
            var successfull = AddMinorCustomerNumberOne(customerManagerMock);
            Assert.AreEqual(AddCustomerStatusCodes.CustomerIsMinor, successfull);
            customerManagerMock.Verify(
                m => m.AddCustomer(It.Is<int>(i => i == 000008), It.Is<string>(i => i == "Pontus Andersson"), It.Is<string>(i => i == "2008-02-16"),
                It.Is<string>(i => i == "Linfrögatan 22"), It.Is<decimal>(i => i == 0), It.IsAny<Customer>(), It.Is<bool>(i => i == false)),
                    Times.Once());

            customerManagerMock.Verify(
                m => m.SetCustomerAsGuardian(It.IsAny<Customer>(), It.Is<bool>(i => i == true)),
                    Times.Once());
        }


        //CANCEL CUSTOMER:
        [TestMethod]
        public void CancelCustomerOk()
        {            
            var customerManagerMock = SetupMock(new Customer
            {
                CustomerNumber = 000001,
                Debt = 0,
                IsGuardian = false                
            });

            var borrowManagerMock = SetupMock(new List<Borrow>());
            
            var customerAPI = new CustomerAPI(customerManagerMock.Object, borrowManagerMock.Object);
            var result = customerAPI.CancelCustomer(000001);
            Assert.AreEqual(CancelCustomerStatusCodes.Ok, result);
            customerManagerMock.Verify(m =>
                m.CancelCustomer(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void CancelCustomerNoSuchCustomer()
        {
            var customerManagerMock = SetupMock((Customer)null);
            var borrowManagerMock = SetupMock(new List<Borrow>());

            var customerAPI = new CustomerAPI(customerManagerMock.Object, borrowManagerMock.Object);
            var result = customerAPI.CancelCustomer(000001);
            Assert.AreEqual(CancelCustomerStatusCodes.NoSuchCustomer, result);
            customerManagerMock.Verify(m =>
                m.CancelCustomer(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void CancelCustomerIsGuardian()
        {
            var customerManagerMock = SetupMock(new Customer
            {
                CustomerNumber = 000001,
                Debt = 0,
                IsGuardian = true
            });

            var borrowManagerMock = SetupMock(new List<Borrow>());

            var customerAPI = new CustomerAPI(customerManagerMock.Object, borrowManagerMock.Object);
            var result = customerAPI.CancelCustomer(000001);
            Assert.AreEqual(CancelCustomerStatusCodes.CustomerIsGuardian, result);
            customerManagerMock.Verify(m =>
                m.CancelCustomer(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void CancelCustomerHasDebt()
        {
            var customerManagerMock = SetupMock(new Customer
            {
                CustomerNumber = 000001,
                Debt = 30,
                IsGuardian = false
            });

            var borrowManagerMock = SetupMock(new List<Borrow>());

            var customerAPI = new CustomerAPI(customerManagerMock.Object, borrowManagerMock.Object);
            var result = customerAPI.CancelCustomer(000001);
            Assert.AreEqual(CancelCustomerStatusCodes.CustomerHasUnpaidDebt, result);
            customerManagerMock.Verify(m =>
                m.CancelCustomer(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void CancelCustomerBorrowsBooks()
        {
            var customerManagerMock = SetupMock(new Customer
            {
                CustomerNumber = 000001,
                Debt = 0,
                IsGuardian = false
            });

            var borrowManagerMock = SetupMock(new List<Borrow>
            {
                new Borrow()
            });

            var customerAPI = new CustomerAPI(customerManagerMock.Object, borrowManagerMock.Object);
            var result = customerAPI.CancelCustomer(000001);
            Assert.AreEqual(CancelCustomerStatusCodes.CustomerHasBorrowedBooks, result);
            customerManagerMock.Verify(m =>
                m.CancelCustomer(It.IsAny<int>()), Times.Never);
        }


        //SETUPMOCKS:
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

        private static Mock<IBorrowManager> SetupMock(List<Borrow> borrow)
        {
            var borrowManagerMock = new Mock<IBorrowManager>();
            borrowManagerMock.Setup(m =>
                    m.GetAllCurrentBorrowFromCustomer(It.IsAny<int>()))
                .Returns(borrow);
            return borrowManagerMock;
        }
    }
}
