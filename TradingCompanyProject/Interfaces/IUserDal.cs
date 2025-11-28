using TradingCompanyDto;

namespace TradingCompanyDal.Interfaces
{
    public interface IUserDal
    {
        User GetUserByUsername(string username);
        void AddUser(User user);
    }
}