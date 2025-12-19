using TradingCompanyDto;

public interface IOrderManager
{
    IEnumerable<Order> GetFilteredOrders(DateTime? start, DateTime? end, int? statusId);
    void UpdateOrder(Order order, int userId);
}