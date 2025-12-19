using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TradingCompanyDal.Interfaces;
using TradingCompanyDto;
using TradingCompanyWeb.Controllers;
using TradingCompanyWeb.Models;

namespace TradingCompanyTests
{
    [TestFixture]
    public class OrdersControllerTests
    {
        private Mock<IOrderManager> _managerMock;
        private Mock<IOrderDal> _dalMock;
        private Mock<IStatusDal> _statusDalMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<OrdersController>> _loggerMock;
        private OrdersController _controller;

        [SetUp]
        public void Setup()
        {
            _managerMock = new Mock<IOrderManager>();
            _dalMock = new Mock<IOrderDal>();
            _statusDalMock = new Mock<IStatusDal>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrdersController>>();

            _controller = new OrdersController(
                _managerMock.Object,
                _dalMock.Object,
                _statusDalMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);

        
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "test_seller"),
                new Claim(ClaimTypes.Role, "Seller"),
                new Claim("UserId", "4")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [TearDown]
        public void Cleanup()
        {
            _controller?.Dispose();
        }





        [Test]
        public void Index_ReturnsViewResult_WithAllOrders()
        {
            // Arrange
            var orders = new List<Order> { new Order { OrderId = 101 } };
            _dalMock.Setup(d => d.GetAll()).Returns(orders);
            _statusDalMock.Setup(s => s.GetAll()).Returns(new List<Status>());

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _mapperMock.Verify(m => m.Map<IEnumerable<OrderViewModel>>(orders), Times.Once);
        }

        [Test]
        public void Details_ExistingId_ReturnsViewWithModel()
        {
            // Arrange
            int id = 1;
            var order = new Order { OrderId = id, StatusId = 1 };
            var vm = new OrderViewModel { OrderId = id };

            _dalMock.Setup(d => d.GetById(id)).Returns(order);
            _statusDalMock.Setup(s => s.GetById(1)).Returns(new Status { StatusName = "Paid" });
            _mapperMock.Setup(m => m.Map<OrderViewModel>(order)).Returns(vm);

            // Act
            var result = _controller.Details(id) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(vm));
        }







        [Test]
        public void Create_Get_ReturnsViewWithStatuses()
        {
            // Arrange
            _statusDalMock.Setup(s => s.GetAll()).Returns(new List<Status>());

            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _statusDalMock.Verify(s => s.GetAll(), Times.Once);
        }

        [Test]
        public void Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var vm = new OrderViewModel { CustomerId = 1, OrderDate = DateTime.Now.AddDays(-1), TotalAmount = 50 };
            var order = new Order();
            _mapperMock.Setup(m => m.Map<Order>(vm)).Returns(order);

            // Act
            var result = _controller.Create(vm) as RedirectToActionResult;

            // Assert
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _dalMock.Verify(d => d.Create(order), Times.Once);
        }

        [Test]
        public void Create_Post_FutureDate_ReturnsViewWithError()
        {
            // Arrange
            var vm = new OrderViewModel { OrderDate = DateTime.Now.AddDays(10) };


            _controller.ModelState.AddModelError("OrderDate", "Order date cannot be in the future.");

            // Act
            var result = _controller.Create(vm) as ViewResult;

            // Assert
            Assert.That(_controller.ModelState.ContainsKey("OrderDate"), Is.True);
            Assert.That(result, Is.Not.Null);
        }

    





        [Test]
        public void Edit_Post_ValidModel_CallsManagerAndUpdate()
        {
            // Arrange
            int id = 5;
            var vm = new OrderViewModel { OrderId = id, OrderDate = DateTime.Now };
            var order = new Order { OrderId = id };
            _mapperMock.Setup(m => m.Map<Order>(vm)).Returns(order);

            // Act
            var result = _controller.Edit(id, vm) as RedirectToActionResult;

            // Assert
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _managerMock.Verify(m => m.UpdateOrder(order, 4), Times.Once);
        }

        [Test]
        public void Edit_Post_DatabaseException_AddsModelError()
        {
            // Arrange
            int id = 5;
            var vm = new OrderViewModel { OrderId = id };
            _mapperMock.Setup(m => m.Map<Order>(vm)).Returns(new Order());
            _managerMock.Setup(m => m.UpdateOrder(It.IsAny<Order>(), It.IsAny<int>()))
                        .Throws(new System.Exception("DB Error"));

            // Act
            var result = _controller.Edit(id, vm) as ViewResult;

            // Assert
            Assert.That(_controller.ModelState.IsValid, Is.False);
            _loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<System.Exception>(), It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()), Times.Once);
        }






        [Test]
        public void Delete_Get_ReturnsNotFound_IfOrderNull()
        {
            // Arrange
            _dalMock.Setup(d => d.GetById(It.IsAny<int>())).Returns((Order)null);

            // Act
            var result = _controller.Delete(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void DeleteConfirmed_CallsDalDelete()
        {
            // Arrange
            int id = 10;

            // Act
            var result = _controller.DeleteConfirmed(id) as RedirectToActionResult;

            // Assert
            _dalMock.Verify(d => d.Delete(id), Times.Once);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

    }
}