using TradingCompanyDal.Interfaces;
using TradingCompanyDto;
using Microsoft.Data.SqlClient;

namespace TradingCompanyDal.Concrete
{
  
        public class StatusDal : IStatusDal
        {
            private readonly string _connectionString = "Data Source=localhost;Initial Catalog=Software;Integrated Security=True;TrustServerCertificate=True";

        public Status Create(Status status)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Statuses (StatusName) OUTPUT inserted.StatusId VALUES (@name)";
                command.Parameters.AddWithValue("@name", status.StatusName);

                status.StatusId = (int)command.ExecuteScalar();
                return status;
            }

            public List<Status> GetAll()
            {
                List<Status> statuses = new List<Status>();
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Statuses";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    statuses.Add(new Status
                    {
                        StatusId = (int)reader["StatusId"],
                        StatusName = (string)reader["StatusName"]
                    });
                }
                return statuses;
            }

            public Status GetById(int statusId)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Statuses WHERE StatusId = @id";
                command.Parameters.AddWithValue("@id", statusId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Status
                    {
                        StatusId = (int)reader["StatusId"],
                        StatusName = (string)reader["StatusName"]
                    };
                }
                return null;
            }

            public Status Update(Status status)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Statuses SET StatusName=@name WHERE StatusId=@id";
                command.Parameters.AddWithValue("@name", status.StatusName);
                command.Parameters.AddWithValue("@id", status.StatusId);

                command.ExecuteNonQuery();
                return status;
            }

            public bool Delete(int statusId)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Statuses WHERE StatusId=@id";
                command.Parameters.AddWithValue("@id", statusId);

                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
        }
    
}
