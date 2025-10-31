using TradingCompanyDal.Interfaces;
using TradingCompanyDto;
using Microsoft.Data.SqlClient;

namespace TradingCompanyDal.Concrete
{
  
        public class OrderDetailDal : IOrderDetailDal
        {
            private readonly string _connectionString = "Data Source=localhost;Initial Catalog=Software;Integrated Security=True;TrustServerCertificate=True";

        public OrderDetail Create(OrderDetail orderDetail)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO OrderDetails (OrderId, ProductId, Quantity) OUTPUT inserted.OrderDetailId VALUES (@orderId, @productId, @quantity)";
                command.Parameters.AddWithValue("@orderId", orderDetail.OrderId);
                command.Parameters.AddWithValue("@productId", orderDetail.ProductId);
                command.Parameters.AddWithValue("@quantity", orderDetail.Quantity);

                orderDetail.OrderDetailId = (int)command.ExecuteScalar();
                return orderDetail;
            }

            public List<OrderDetail> GetAll()
            {
                List<OrderDetail> details = new List<OrderDetail>();
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM OrderDetails";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    details.Add(new OrderDetail
                    {
                        OrderDetailId = (int)reader["OrderDetailId"],
                        OrderId = (int)reader["OrderId"],
                        ProductId = (int)reader["ProductId"],
                        Quantity = (int)reader["Quantity"]
                    });
                }
                return details;
            }

            public OrderDetail GetById(int orderDetailId)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM OrderDetails WHERE OrderDetailId = @id";
                command.Parameters.AddWithValue("@id", orderDetailId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new OrderDetail
                    {
                        OrderDetailId = (int)reader["OrderDetailId"],
                        OrderId = (int)reader["OrderId"],
                        ProductId = (int)reader["ProductId"],
                        Quantity = (int)reader["Quantity"]
                    };
                }
                return null;
            }

            public OrderDetail Update(OrderDetail orderDetail)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET OrderId=@orderId, ProductId=@productId, Quantity=@quantity WHERE OrderDetailId=@id";
                command.Parameters.AddWithValue("@orderId", orderDetail.OrderId);
                command.Parameters.AddWithValue("@productId", orderDetail.ProductId);
                command.Parameters.AddWithValue("@quantity", orderDetail.Quantity);
                command.Parameters.AddWithValue("@id", orderDetail.OrderDetailId);

                command.ExecuteNonQuery();
                return orderDetail;
            }

            public bool Delete(int orderDetailId)
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM OrderDetails WHERE OrderDetailId=@id";
                command.Parameters.AddWithValue("@id", orderDetailId);

                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
        }
    
}
