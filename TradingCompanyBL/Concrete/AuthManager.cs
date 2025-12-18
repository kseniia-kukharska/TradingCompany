using System.Security.Cryptography;
using System.Text;
using TradingCompanyBL.Interfaces;
using TradingCompanyDal.Interfaces;
using TradingCompanyDto;

namespace TradingCompanyBL.Concrete
{
    public class AuthManager : IAuthManager
    {
        private readonly IUserDal _userDal;

 
        public AuthManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public User Login(string username, string password)
        {
  
            var user = _userDal.GetUserByUsername(username);

  
            if (user == null) return null;

            string inputHash = HashPassword(password, user.Salt);

            if (inputHash == user.PasswordHash)
            {
                return user;
            }

            return null;
        }

        public bool Register(string username, string password, int roleId)
        {
            if (_userDal.GetUserByUsername(username) != null)
                return false;

            string salt = GenerateSalt();

            string hash = HashPassword(password, salt);

            var newUser = new User
            {
                Username = username,
                PasswordHash = hash, 
                Salt = salt,        
                RoleId = roleId
            };
            _userDal.AddUser(newUser);
            return true;
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
               
                string combined = password + salt;

               
                byte[] bytes = Encoding.UTF8.GetBytes(combined);
                byte[] hash = sha256.ComputeHash(bytes);

            
                return Convert.ToBase64String(hash);
            }
        }
    }
}