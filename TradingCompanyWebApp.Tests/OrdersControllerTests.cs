using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TradingCompanyBL.Interfaces;
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
        }

        [Test]
        public void Index_ReturnsAllOrders()
        {
            // Arrange
            var orders = new List<Order> { new Order { OrderId = 1 } };
            _dalMock.Setup(d => d.GetAll()).Returns(orders);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            _mapperMock.Verify(m => m.Map<IEnumerable<OrderViewModel>>(orders), Times.Once);
        }

        [Test]
        public void Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Required");
            var model = new OrderViewModel();

            // Act
            var result = _controller.Create(model) as ViewResult;

            // Assert
            Assert.AreEqual(model, result.Model);
        }
    }
}