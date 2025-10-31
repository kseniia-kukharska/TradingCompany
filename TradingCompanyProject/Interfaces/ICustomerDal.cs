using TradingCompanyDto;

namespace TradingCompanyDal.Interfaces
{
    public interface ICustomerDal
    {
        Customer Create(Customer customer);
        List<Customer> GetAll();
        Customer GetById(int customerId);
        Customer Update(Customer customer);
        bool Delete(int customerId);
    }
}
