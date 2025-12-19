using TradingCompanyDal.Interfaces;
using TradingCompanyDto;

namespace TradingCompanyBL.Concrete
{
    public class OrderManager : IOrderManager
    {
        private readonly IOrderDal _orderDal;

        public OrderManager(IOrderDal orderDal)
        {
            _orderDal = orderDal;
        }

        public IEnumerable<Order> GetFilteredOrders(DateTime? start, DateTime? end, int? statusId)
        {
            IEnumerable<Order> orders = _orderDal.GetAll();

            if (start.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= start.Value);
            }

            if (end.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= end.Value);
            }

            if (statusId.HasValue && statusId > 0)
            {
                orders = orders.Where(o => o.StatusId == statusId.Value);
            }
            return orders.ToList();
        }

        public void UpdateOrder(Order order, int userId)
        {
            _orderDal.Update(order);
        }
    }
}