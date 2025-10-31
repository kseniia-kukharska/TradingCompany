namespace TradingCompany2025.Console
{
    using System;
    using System.Linq;
    using TradingCompanyDal.Concrete;
    using TradingCompanyDto;

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Trading Company Product Management!");
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
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        if (c != 's')
                        {
                            Console.WriteLine("Invalid choice. Please try again.");
                        }
                        break;
                }

                Console.WriteLine("Choose option:\n1. Get all Products;");
                Console.WriteLine("2. Insert a Product;");
                Console.WriteLine("3. Get a Product by Id;");
                Console.WriteLine("4. Update a Product;");
                Console.WriteLine("5. Delete a Product;");
                Console.WriteLine("0. Quit.");
                Console.Write("Your choice: ");

               
                string input = Console.ReadLine();
                c = input.Length > 0 ? input[0] : ' ';
            }
        }

        private static void PrintProduct(Product product)
        {
            if (product != null)
            {
                Console.WriteLine($"\tId: {product.ProductId}, Name: {product.Name}, Price: {product.Price}, Amount: {product.Amount}");
            }
            else
            {
                Console.WriteLine("\tProduct not found.");
            }
        }

        private static void GetAllProducts()
        {
            try
            {
                var dal = new ProductDal();
                var products = dal.GetAll();

                if (products.Any())
                {
                    Console.WriteLine($"Found {products.Count} products:");
                    foreach (var product in products)
                    {
                        PrintProduct(product);
                    }
                }
                else
                {
                    Console.WriteLine("No products found in the database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving products: {ex.Message}");
            }
        }

        private static void InsertProduct()
        {
            try
            {
                
                Console.Write("Enter product Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter product Price: ");
   
                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.WriteLine("Invalid price format. Insertion cancelled.");
                    return;
                }

                Console.Write("Enter product Amount: ");
                
                if (!int.TryParse(Console.ReadLine(), out int amount))
                {
                    Console.WriteLine("Invalid amount format. Insertion cancelled.");
                    return;
                }

                var dal = new ProductDal();

                var newProduct = new Product
                {
                    Name = name,
                    Price = price,
                    Amount = amount
                };

                var createdProduct = dal.Create(newProduct);

                Console.Write("Successfully Inserted Product: ");
                PrintProduct(createdProduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while inserting product: {ex.Message}");
            }
        }

        private static void GetProductById()
        {
            try
            {
                Console.Write("Enter Product Id: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid Id format.");
                    return;
                }

                var dal = new ProductDal();
                var product = dal.GetById(id);

                Console.Write($"Product with Id {id}: ");
                PrintProduct(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving product: {ex.Message}");
            }
        }

        private static void UpdateProduct()
        {
            try
            {
                Console.Write("Enter Product Id: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid Id format. Update cancelled.");
                    return;
                }

                var dal = new ProductDal();
                var existingProduct = dal.GetById(id);

                if (existingProduct == null)
                {
                    Console.WriteLine($"Product with Id {id} not found.");
                    return;
                }

                Console.WriteLine("Current Product Details:");
                PrintProduct(existingProduct);
                Console.WriteLine("Enter new values (leave empty to keep current value):");

                Console.Write($"Enter new Name ({existingProduct.Name}): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    existingProduct.Name = newName;
                }

         
                Console.Write($"Enter new Price ({existingProduct.Price}): ");
                string newPriceStr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPriceStr) && decimal.TryParse(newPriceStr, out decimal newPrice))
                {
                    existingProduct.Price = newPrice;
                }

                Console.Write($"Enter new Amount ({existingProduct.Amount}): ");
                string newAmountStr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAmountStr) && int.TryParse(newAmountStr, out int newAmount))
                {
                    existingProduct.Amount = newAmount;
                }

                var updatedProduct = dal.Update(existingProduct);
                Console.Write("Successfully Updated Product: ");
                PrintProduct(updatedProduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating product: {ex.Message}");
            }
        }

        private static void DeleteProduct()
        {
            try
            {
                Console.Write("Enter Product Id: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid Id format. Deletion cancelled.");
                    return;
                }

                var dal = new ProductDal();
                bool success = dal.Delete(id);

                if (success)
                {
                    Console.WriteLine($"Successfully deleted Product with Id {id}.");
                }
                else
                {
                    Console.WriteLine($"Product with Id {id} not found or could not be deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting product: {ex.Message}");
            }
        }
    }
}