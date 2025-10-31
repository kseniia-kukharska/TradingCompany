using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingCompanyDto;

namespace TradingCompanyDal.Interfaces
{
    public interface IOrderDetailDal
    {
        OrderDetail Create(OrderDetail orderDetail);
        List<OrderDetail> GetAll();
        OrderDetail GetById(int orderDetailId);
        OrderDetail Update(OrderDetail orderDetail);
        bool Delete(int orderDetailId);

    }
}
