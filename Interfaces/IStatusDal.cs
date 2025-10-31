using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingCompanyDto;

namespace TradingCompanyDal.Interfaces
{
    public interface IStatusDal
    {
        Status Create(Status status);
        List<Status> GetAll();
        Status GetById(int statusId);
        Status Update(Status status);
        bool Delete(int statusId);
    }
}
