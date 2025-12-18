using System;
using System.Collections.Generic;
using System.Linq;
using TradingCompanyBL.Interfaces;
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
            // КЛЮЧОВЕ ВИПРАВЛЕННЯ: вказуємо тип IEnumerable<Order> явно.
            // Тепер ми можемо фільтрувати дані без постійного виклику .ToList()
            IEnumerable<Order> orders = _orderDal.GetAll();

            if (start.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= start.Value);
            }

            if (end.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= end.Value);
            }

            // Використовуємо .Value тільки всередині перевірки .HasValue
            if (statusId.HasValue && statusId > 0)
            {
                orders = orders.Where(o => o.StatusId == statusId.Value);
            }

            // Перетворюємо в список тільки один раз у самому кінці
            return orders.ToList();
        }

        public void UpdateOrderWithHistory(Order order, int userId)
        {
            // Оновлюємо замовлення в базі даних
            _orderDal.Update(order);

            // Тут ви можете додати логіку для IOrderHistoryDal, якщо вона реалізована
            // Наприклад: 
            // var history = new OrderHistory { OrderId = order.OrderId, UserId = userId, ChangeDate = DateTime.Now };
            // _historyDal.Add(history);
        }
    }
}