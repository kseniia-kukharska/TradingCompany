using TradingCompanyDal.Interfaces;
using TradingCompanyDto;
using Microsoft.Data.SqlClient;

namespace TradingCompanyDal.Concrete
{
        public class OrderDal : IOrderDal
        {
            private readonly string _connectionString = "Data Source=localhost;Initial Catalog=Software;Integrated Security=True;TrustServerCertificate=True";

        public Order Create(Order order)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Orders (CustomerId, StatusId, TotalAmount, OrderDate) OUTPUT inserted.OrderId VALUES (@customerId, @statusId, @totalAmount, @orderDate)";
                command.Parameters.AddWithValue("@customerId", order.CustomerId);
                command.Parameters.AddWithValue("@statusId", order.StatusId);
                command.Parameters.AddWithValue("@totalAmount", order.TotalAmount);
                command.Parameters.AddWithValue("@orderDate", order.OrderDate);

                order.OrderId = (int)command.ExecuteScalar();
                return order;
            }

            public List<Order> GetAll()
            {
                List<Order> orders = new List<Order>();
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Orders";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = (int)reader["OrderId"],
                        CustomerId = (int)reader["CustomerId"],
                        StatusId = (int)reader["StatusId"],
                        TotalAmount = (decimal)reader["TotalAmount"],
                        OrderDate = (DateTime)reader["OrderDate"]
                    });
                }
                return orders;
            }

            public Order GetById(int orderId)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Orders WHERE OrderId = @id";
                command.Parameters.AddWithValue("@id", orderId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Order
                    {
                        OrderId = (int)reader["OrderId"],
                        CustomerId = (int)reader["CustomerId"],
                        StatusId = (int)reader["StatusId"],
                        TotalAmount = (decimal)reader["TotalAmount"],
                        OrderDate = (DateTime)reader["OrderDate"]
                    };
                }
                return null;
            }

            public Order Update(Order order)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Orders SET CustomerId=@customerId, StatusId=@statusId, TotalAmount=@totalAmount, OrderDate=@orderDate WHERE OrderId=@id";
                command.Parameters.AddWithValue("@customerId", order.CustomerId);
                command.Parameters.AddWithValue("@statusId", order.StatusId);
                command.Parameters.AddWithValue("@totalAmount", order.TotalAmount);
                command.Parameters.AddWithValue("@orderDate", order.OrderDate);
                command.Parameters.AddWithValue("@id", order.OrderId);

                command.ExecuteNonQuery();
                return order;
            }

            public bool Delete(int orderId)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Orders WHERE OrderId=@id";
                command.Parameters.AddWithValue("@id", orderId);

                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
        }
    
}
