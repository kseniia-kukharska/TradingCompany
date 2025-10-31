using TradingCompanyDal.Concrete;
using Product = TradingCompanyDto.Product;


namespace TradingCompanyTests;

public class ProductDalTest
{
    public class ProductDalTests
    {
        // Рядок підключення використовується безпосередньо з вашого класу ProductDal
        // private readonly string _connStr = "Data Source=localhost;Initial Catalog=Software;Integrated Security=True; TrustServerCertificate=True";
        private ProductDal _dal;
        private List<Product> _testProducts;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            // ProductDal приймає connection string через поле, тому ми просто ініціалізуємо клас.
            _dal = new ProductDal();
            _testProducts = new List<Product>();
        }

        [SetUp]
        public void Setup()
        {
            // Arrange: Створення нового унікального тестового продукту перед кожним тестом
            var product = new Product
            {
                Name = $"Test Product {System.Guid.NewGuid()}",
                Price = 99.99m,
                Amount = 10
            };

            // Act: Викликаємо метод Create
            var created = _dal.Create(product);

            // Assert: Переконуємось, що продукт має Id
            Assert.That(created.ProductId, Is.GreaterThan(0), "Create не повернув коректний ProductId");

            // Зберігаємо для очищення (TearDown)
            _testProducts.Add(created);
        }

        [TearDown]
        public void Cleanup()
        {
            // Видаляємо всі створені тестові продукти після кожного тесту
            foreach (var product in _testProducts)
            {
                if (product.ProductId > 0)
                {
                    _dal.Delete(product.ProductId);
                }
            }
            _testProducts.Clear();
        }

        // --- Тести для CRUD операцій ---

        [Test]
        public void CreateProduct_ShouldReturnNewProductWithId()
        {
            // Arrange: Створення абсолютно нового продукту для цього тесту
            var product = new Product
            {
                Name = "Unique New Product",
                Price = 149.50m,
                Amount = 5
            };

            // Act: Викликаємо Create
            var created = _dal.Create(product);

            // Зберігаємо для очищення, хоча можемо видалити одразу
            _testProducts.Add(created);

            // Assert
            Assert.That(created.ProductId, Is.GreaterThan(0), "New product must have an Id");
            Assert.That(created.Name, Is.EqualTo(product.Name));
        }

        [Test]
        public void GetAll_ShouldReturnProducts()
        {
            // Setup вже створив один продукт, тому очікуємо принаймні один.

            // Act
            var products = _dal.GetAll();

            // Assert
            Assert.That(products, Is.Not.Null);
            // Перевіряємо, що в базі є принаймні наш тестовий продукт
            Assert.That(products.Count, Is.GreaterThanOrEqualTo(1));
            // Перевіряємо, що наш продукт є у списку
            var ourProduct = products.FirstOrDefault(p => p.ProductId == _testProducts[0].ProductId);
            Assert.That(ourProduct, Is.Not.Null);
        }

        [Test]
        public void GetById_ShouldReturnCorrectProduct()
        {
            // Arrange: Беремо Id продукту, створеного у Setup
            var expectedProduct = _testProducts[0];

            // Act
            var actualProduct = _dal.GetById(expectedProduct.ProductId);

            // Assert
            Assert.That(actualProduct, Is.Not.Null);
            Assert.That(actualProduct.ProductId, Is.EqualTo(expectedProduct.ProductId));
            Assert.That(actualProduct.Name, Is.EqualTo(expectedProduct.Name));
            Assert.That(actualProduct.Price, Is.EqualTo(expectedProduct.Price));
            Assert.That(actualProduct.Amount, Is.EqualTo(expectedProduct.Amount));
        }

        [Test]
        public void GetById_ShouldReturnNullForNonExistentId()
        {
            // Arrange
            const int nonExistentId = -1;

            // Act
            var product = _dal.GetById(nonExistentId);

            // Assert
            Assert.That(product, Is.Null);
        }

        [Test]
        public void UpdateProduct_ShouldChangeValuesInDatabase()
        {
            // Arrange
            var productToUpdate = _testProducts[0];
            const string newName = "Updated Product Name";
            const decimal newPrice = 4.50m;
            const int newAmount = 100;

            productToUpdate.Name = newName;
            productToUpdate.Price = newPrice;
            productToUpdate.Amount = newAmount;

            // Act: Оновлюємо та отримуємо оновлений об'єкт
            var updated = _dal.Update(productToUpdate);
            // Act: Отримуємо об'єкт безпосередньо з бази для верифікації
            var fromDb = _dal.GetById(productToUpdate.ProductId);

            // Assert
            Assert.That(updated.Name, Is.EqualTo(newName), "Return value name mismatch");
            Assert.That(fromDb.Name, Is.EqualTo(newName), "DB value name mismatch");
            Assert.That(fromDb.Price, Is.EqualTo(newPrice), "DB value price mismatch");
            Assert.That(fromDb.Amount, Is.EqualTo(newAmount), "DB value amount mismatch");
        }

        [Test]
        public void DeleteProduct_ShouldRemoveFromDatabase()
        {
            // Arrange: Створюємо продукт, який одразу видалимо
            var productToDelete = new Product
            {
                Name = "Product To Delete",
                Price = 1.0m,
                Amount = 1
            };
            var created = _dal.Create(productToDelete);
            int idToDelete = created.ProductId;

            // Act
            bool result = _dal.Delete(idToDelete);
            var check = _dal.GetById(idToDelete); // Перевіряємо наявність

            // Assert
            Assert.That(result, Is.True, "Delete method should return true on success");
            Assert.That(check, Is.Null, "Product should not be found after deletion");
        }
    }
}