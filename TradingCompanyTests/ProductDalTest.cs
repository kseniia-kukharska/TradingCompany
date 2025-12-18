using TradingCompanyDal.Concrete;
using Product = TradingCompanyDto.Product;

namespace TradingCompanyTests;

public class ProductDalTest
{
    public class ProductDalTests
    {
        private ProductDal _dal;
        private List<Product> _testProducts;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=Software;Integrated Security=True; TrustServerCertificate=True";
            _dal = new ProductDal(connectionString);
            _testProducts = new List<Product>();
        }

        [SetUp]
        public void Setup()
        {
            var product = new Product
            {
                Name = $"Test Product {System.Guid.NewGuid()}",
                Price = 99.99m,
                Amount = 10
            };

            var created = _dal.Create(product);

            Assert.That(created.ProductId, Is.GreaterThan(0), "Create didn't return correct ProductId");

            _testProducts.Add(created);
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var product in _testProducts)
            {
                if (product.ProductId > 0)
                {
                    _dal.Delete(product.ProductId);
                }
            }
            _testProducts.Clear();
        }

        [Test]
        public void CreateProduct_ShouldReturnNewProductWithId()
        {
            var product = new Product
            {
                Name = "Unique New Product",
                Price = 149.50m,
                Amount = 5
            };

            var created = _dal.Create(product);

            _testProducts.Add(created);

            Assert.That(created.ProductId, Is.GreaterThan(0), "New product must have an Id");
            Assert.That(created.Name, Is.EqualTo(product.Name));
        }

        [Test]
        public void GetAll_ShouldReturnProducts()
        {
            var products = _dal.GetAll();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.GreaterThanOrEqualTo(1));

            var ourProduct = products.FirstOrDefault(p => p.ProductId == _testProducts[0].ProductId);
            Assert.That(ourProduct, Is.Not.Null);
        }

        [Test]
        public void GetById_ShouldReturnCorrectProduct()
        {
            var expectedProduct = _testProducts[0];

            var actualProduct = _dal.GetById(expectedProduct.ProductId);

            Assert.That(actualProduct, Is.Not.Null);
            Assert.That(actualProduct.ProductId, Is.EqualTo(expectedProduct.ProductId));
            Assert.That(actualProduct.Name, Is.EqualTo(expectedProduct.Name));
            Assert.That(actualProduct.Price, Is.EqualTo(expectedProduct.Price));
            Assert.That(actualProduct.Amount, Is.EqualTo(expectedProduct.Amount));
        }

        [Test]
        public void GetById_ShouldReturnNullForNonExistentId()
        {
            const int nonExistentId = -1;

            var product = _dal.GetById(nonExistentId);

            Assert.That(product, Is.Null);
        }

        [Test]
        public void UpdateProduct_ShouldChangeValuesInDatabase()
        {
            var productToUpdate = _testProducts[0];
            const string newName = "Updated Product Name";
            const decimal newPrice = 4.50m;
            const int newAmount = 100;

            productToUpdate.Name = newName;
            productToUpdate.Price = newPrice;
            productToUpdate.Amount = newAmount;

            var updated = _dal.Update(productToUpdate);
            var fromDb = _dal.GetById(productToUpdate.ProductId);

            Assert.That(updated.Name, Is.EqualTo(newName), "Return value name mismatch");
            Assert.That(fromDb.Name, Is.EqualTo(newName), "DB value name mismatch");
            Assert.That(fromDb.Price, Is.EqualTo(newPrice), "DB value price mismatch");
            Assert.That(fromDb.Amount, Is.EqualTo(newAmount), "DB value amount mismatch");
        }

        [Test]
        public void DeleteProduct_ShouldRemoveFromDatabase()
        {
            var productToDelete = new Product
            {
                Name = "Product To Delete",
                Price = 1.0m,
                Amount = 1
            };
            var created = _dal.Create(productToDelete);
            int idToDelete = created.ProductId;

            bool result = _dal.Delete(idToDelete);
            var check = _dal.GetById(idToDelete);

            Assert.That(result, Is.True, "Delete method should return true on success");
            Assert.That(check, Is.Null, "Product should not be found after deletion");
        }
    }
}