using TradingCompanyDto;

public interface IOrderManager
{
    IEnumerable<Order> GetFilteredOrders(DateTime? start, DateTime? end, int? statusId);
    void UpdateOrderWithHistory(Order order, int userId);
}