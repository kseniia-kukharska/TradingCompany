
namespace TradingCompanyDto
{ 
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } 
    }
}