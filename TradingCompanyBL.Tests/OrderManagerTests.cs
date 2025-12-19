using Moq;
using TradingCompanyBL.Concrete;
using TradingCompanyDal.Interfaces;
using TradingCompanyDto;

namespace TradingCompanyBL.Tests
{
    [TestFixture]
    public class OrderManagerTests
    {
        private Mock<IOrderDal> _orderDalMock;
        private OrderManager _orderManager;
        private List<Order> _sampleOrders;

        [SetUp]
        public void Setup()
        {
            _orderDalMock = new Mock<IOrderDal>();
            _sampleOrders = new List<Order>
            {
                new Order { OrderId = 1, OrderDate = new DateTime(2025, 01, 01), StatusId = 1, TotalAmount = 100 },
                new Order { OrderId = 2, OrderDate = new DateTime(2025, 02, 01), StatusId = 2, TotalAmount = 200 },
                new Order { OrderId = 3, OrderDate = new DateTime(2025, 03, 01), StatusId = 1, TotalAmount = 300 }
            };

            _orderDalMock.Setup(dal => dal.GetAll()).Returns(_sampleOrders);

            _orderManager = new OrderManager(_orderDalMock.Object);
        }

        [Test]
        public void GetFilteredOrders_WhenNoFilters_ReturnsAllOrders()
        {
            // Act
            var result = _orderManager.GetFilteredOrders(null, null, null);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(3), "Should return all orders when no filters are applied.");
        }

        [Test]
        public void GetFilteredOrders_FilterByStatus_ReturnsOnlyCorrectStatus()
        {
            // Act
            var result = _orderManager.GetFilteredOrders(null, null, 2);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1), "Result count mismatch for status filter.");
            Assert.That(result.First().OrderId, Is.EqualTo(2), "The filtered order has the wrong ID.");
        }

        [Test]
        public void GetFilteredOrders_FilterByDateRange_ReturnsCorrectOrders()
        {
            // Arrange
            DateTime start = new DateTime(2025, 01, 15);
            DateTime end = new DateTime(2025, 02, 15);

            // Act
            var result = _orderManager.GetFilteredOrders(start, end, null);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1), "Result count mismatch for date range filter.");
            Assert.That(result.First().OrderId, Is.EqualTo(2), "The filtered order date is outside the requested range.");
        }

        [Test]
        public void GetFilteredOrders_WhenStatusIsZero_ShouldIgnoreStatusFilter()
        {
            // Act
            var result = _orderManager.GetFilteredOrders(null, null, 0);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(3), "Filter should be ignored when status ID is zero.");
        }
    }
}