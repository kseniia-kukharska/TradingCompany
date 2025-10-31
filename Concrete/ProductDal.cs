using TradingCompanyDal.Interfaces;
using TradingCompanyDto;
using Microsoft.Data.SqlClient;

namespace TradingCompanyDal.Concrete
{
    public class ProductDal : IProductDal
    {
        private readonly string _connectionString = "Data Source=localhost;Initial Catalog=Software;Integrated Security=True; TrustServerCertificate=True";
        
        public Product Create(Product product)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Products (Name, Price, Amount) OUTPUT inserted.ProductId VALUES (@name, @price, @amount)";
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@price", product.Price);
            command.Parameters.AddWithValue("@amount", product.Amount);

            product.ProductId = (int)command.ExecuteScalar();
            return product;
        }

        public List<Product> GetAll()
        {
            List<Product> products = new List<Product>();
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Products";
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    ProductId = (int)reader["ProductId"],
                    Name = (string)reader["Name"],
                    Price = (decimal)reader["Price"],
                    Amount = (int)reader["Amount"]
                });
            }
            return products;
        }

        public Product GetById(int productId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Products WHERE ProductId = @id";
            command.Parameters.AddWithValue("@id", productId);

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                {
                    ProductId = (int)reader["ProductId"],
                    Name = (string)reader["Name"],
                    Price = (decimal)reader["Price"],
                    Amount = (int)reader["Amount"]
                };
            }
            return null;
        }

        public Product Update(Product product)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Products SET Name=@name, Price=@price, Amount=@amount WHERE ProductId=@id";
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@price", product.Price);
            command.Parameters.AddWithValue("@amount", product.Amount);
            command.Parameters.AddWithValue("@id", product.ProductId);

            command.ExecuteNonQuery();
            return product;
        }

        public bool Delete(int productId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Products WHERE ProductId=@id";
            command.Parameters.AddWithValue("@id", productId);

            int rows = command.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
