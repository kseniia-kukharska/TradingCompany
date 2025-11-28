using Microsoft.Extensions.Configuration;
using TradingCompanyDal.Concrete;
using TradingCompanyDto;

namespace TradingCompanyConsole 
{
    internal class Program
    {
        private static string _connectionString;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            _connectionString = config.GetConnectionString("DefaultConnection");


            if (string.IsNullOrEmpty(_connectionString))
            {
                System.Console.WriteLine("Error: Connection string 'DefaultConnection' not found in appsettings.json.");
                return;
            }

            System.Console.WriteLine("Welcome to Trading Company Product Management!");
            char c = 's';

            while (c != '0')
            {
                switch (c)
                {
                    case '1':
                        GetAllProducts();
                        break;
                    case '2':
                        InsertProduct();
                        break;
                    case '3':
                        GetProductById();
                        break;
                    case '4':
                        UpdateProduct();
                        break;
                    case '5':
                        DeleteProduct();
                        break;
                    case '0':
                        System.Console.WriteLine("Goodbye!");
                        break;
                    default:
                        if (c != 's')
                        {
                            System.Console.WriteLine("Invalid choice. Please try again.");
                        }
                        break;
                }

                System.Console.WriteLine("\nChoose option:");
                System.Console.WriteLine("1. Get all Products;");
                System.Console.WriteLine("2. Insert a Product;");
                System.Console.WriteLine("3. Get a Product by Id;");
                System.Console.WriteLine("4. Update a Product;");
                System.Console.WriteLine("5. Delete a Product;");
                System.Console.WriteLine("0. Quit.");
                System.Console.Write("Your choice: ");

                string input = System.Console.ReadLine();
                c = input.Length > 0 ? input[0] : ' ';
            }
        }

        private static void PrintProduct(Product product)
        {
            if (product != null)
            {
                System.Console.WriteLine($"\tId: {product.ProductId}, Name: {product.Name}, Price: {product.Price}, Amount: {product.Amount}");
            }
            else
            {
                System.Console.WriteLine("\tProduct not found.");
            }
        }

        private static void GetAllProducts()
        {
            try
            {
                var dal = new ProductDal(_connectionString);
                var products = dal.GetAll();

                if (products.Any())
                {
                    System.Console.WriteLine($"Found {products.Count} products:");
                    foreach (var product in products)
                    {
                        PrintProduct(product);
                    }
                }
                else
                {
                    System.Console.WriteLine("No products found in the database.");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"An error occurred while retrieving products: {ex.Message}");
            }
        }

        private static void InsertProduct()
        {
            try
            {
                System.Console.Write("Enter product Name: ");
                string name = System.Console.ReadLine();

                System.Console.Write("Enter product Price: ");
                if (!decimal.TryParse(System.Console.ReadLine(), out decimal price))
                {
                    System.Console.WriteLine("Invalid price format. Insertion cancelled.");
                    return;
                }

                System.Console.Write("Enter product Amount: ");
                if (!int.TryParse(System.Console.ReadLine(), out int amount))
                {
                    System.Console.WriteLine("Invalid amount format. Insertion cancelled.");
                    return;
                }
                var dal = new ProductDal(_connectionString);

                var newProduct = new Product
                {
                    Name = name,
                    Price = price,
                    Amount = amount
                };

                var createdProduct = dal.Create(newProduct);

                System.Console.Write("Successfully Inserted Product: ");
                PrintProduct(createdProduct);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"An error occurred while inserting product: {ex.Message}");
            }
        }

        private static void GetProductById()
        {
            try
            {
                System.Console.Write("Enter Product Id: ");
                if (!int.TryParse(System.Console.ReadLine(), out int id))
                {
                    System.Console.WriteLine("Invalid Id format.");
                    return;
                }

                var dal = new ProductDal(_connectionString);
                var product = dal.GetById(id);

                System.Console.Write($"Product with Id {id}: ");
                PrintProduct(product);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"An error occurred while retrieving product: {ex.Message}");
            }
        }

        private static void UpdateProduct()
        {
            try
            {
                System.Console.Write("Enter Product Id: ");
                if (!int.TryParse(System.Console.ReadLine(), out int id))
                {
                    System.Console.WriteLine("Invalid Id format. Update cancelled.");
                    return;
                }

                var dal = new ProductDal(_connectionString);
                var existingProduct = dal.GetById(id);

                if (existingProduct == null)
                {
                    System.Console.WriteLine($"Product with Id {id} not found.");
                    return;
                }

                System.Console.WriteLine("Current Product Details:");
                PrintProduct(existingProduct);
                System.Console.WriteLine("Enter new values (leave empty to keep current value):");

                System.Console.Write($"Enter new Name ({existingProduct.Name}): ");
                string newName = System.Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    existingProduct.Name = newName;
                }

                System.Console.Write($"Enter new Price ({existingProduct.Price}): ");
                string newPriceStr = System.Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPriceStr) && decimal.TryParse(newPriceStr, out decimal newPrice))
                {
                    existingProduct.Price = newPrice;
                }

                System.Console.Write($"Enter new Amount ({existingProduct.Amount}): ");
                string newAmountStr = System.Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAmountStr) && int.TryParse(newAmountStr, out int newAmount))
                {
                    existingProduct.Amount = newAmount;
                }

                var updatedProduct = dal.Update(existingProduct);
                System.Console.Write("Successfully Updated Product: ");
                PrintProduct(updatedProduct);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"An error occurred while updating product: {ex.Message}");
            }
        }

        private static void DeleteProduct()
        {
            try
            {
                System.Console.Write("Enter Product Id: ");
                if (!int.TryParse(System.Console.ReadLine(), out int id))
                {
                    System.Console.WriteLine("Invalid Id format. Deletion cancelled.");
                    return;
                }

                var dal = new ProductDal(_connectionString);
                bool success = dal.Delete(id);

                if (success)
                {
                    System.Console.WriteLine($"Successfully deleted Product with Id {id}.");
                }
                else
                {
                    System.Console.WriteLine($"Product with Id {id} not found or could not be deleted.");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"An error occurred while deleting product: {ex.Message}");
            }
        }
    }
}