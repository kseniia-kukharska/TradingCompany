using TradingCompanyDal.Interfaces;
using TradingCompanyDto;
using Microsoft.Data.SqlClient;


namespace TradingCompanyDal.Concrete
{
    public class UserDal : IUserDal
    {
        private readonly string _connectionString;

        public UserDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUserByUsername(string username)
        {
            
            string sql = @"
                SELECT u.UserId, u.Username, u.PasswordHash, u.Salt, u.RoleId, r.RoleName
                FROM Users u
                JOIN Roles r ON u.RoleId = r.RoleId
                WHERE u.Username = @Username";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Username", username);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = (int)reader["UserId"],
                            Username = (string)reader["Username"],
                            PasswordHash = (string)reader["PasswordHash"],
                            Salt = (string)reader["Salt"],
                            RoleId = (int)reader["RoleId"],
                            Role = new Role
                            {
                                RoleId = (int)reader["RoleId"],
                                RoleName = (string)reader["RoleName"]
                            }
                        };
                    }
                }
            }
            return null; 
        }

        public void AddUser(User user)
        {
            string sql = @"
                INSERT INTO Users (Username, PasswordHash, Salt, RoleId)
                VALUES (@Username, @PasswordHash, @Salt, @RoleId)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Salt", user.Salt);
                command.Parameters.AddWithValue("@RoleId", user.RoleId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}