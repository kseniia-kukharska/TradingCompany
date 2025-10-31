using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingCompanyDto;

namespace TradingCompanyDal.Interfaces
{
    public interface IOrderDal
    {
        Order Create(Order order);
        List<Order> GetAll();
        Order GetById(int orderId);
        Order Update(Order order);
        bool Delete(int orderId);

    }
}
