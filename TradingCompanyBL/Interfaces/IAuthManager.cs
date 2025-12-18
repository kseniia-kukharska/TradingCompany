using TradingCompanyDto;

namespace TradingCompanyBL.Interfaces
{
    public interface IAuthManager
    {   
        User Login(string username, string password);

        bool Register(string username, string password, int roleId);
    }
}