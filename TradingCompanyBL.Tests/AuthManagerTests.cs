using Moq;
using System.Security.Cryptography;
using System.Text;
using TradingCompanyBL.Concrete;
using TradingCompanyDal.Interfaces;
using TradingCompanyDto;

namespace TradingCompanyBL.Tests
{
    [TestFixture]
    public class AuthManagerTests
    {
        private Mock<IUserDal> _userDalMock;
        private AuthManager _sut;

        [SetUp]
        public void SetUp()
        {         
            _userDalMock = new Mock<IUserDal>(MockBehavior.Strict);
            _sut = new AuthManager(_userDalMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _userDalMock.VerifyAll();
        }




        [Test]
        public void Register_ShouldReturnTrue_WhenUserDoesNotExist()
        {
            // Arrange
            const string username = "new_user";
            const string password = "password123";
            const int roleId = 1;


            _userDalMock.Setup(d => d.GetUserByUsername(username)).Returns((User)null);

            _userDalMock.Setup(d => d.AddUser(It.IsAny<User>()));

            // Act
            bool result = _sut.Register(username, password, roleId);

            // Assert
            Assert.That(result, Is.True);
            _userDalMock.Verify(d => d.AddUser(It.Is<User>(u =>
                u.Username == username &&
                u.RoleId == roleId &&
                !string.IsNullOrEmpty(u.Salt) &&
                !string.IsNullOrEmpty(u.PasswordHash))), Times.Once);
        }

        [Test]
        public void Register_ShouldReturnFalse_WhenUserAlreadyExists()
        {
            // Arrange
            const string username = "existing_user";
            _userDalMock.Setup(d => d.GetUserByUsername(username)).Returns(new User { Username = username });

            // Act
            bool result = _sut.Register(username, "any_password", 1);

            // Assert
            Assert.That(result, Is.False);
            _userDalMock.Verify(d => d.AddUser(It.IsAny<User>()), Times.Never);
        }





        [Test]
        public void Login_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            const string username = "admin";
            const string password = "correct_password";
            const string salt = "random_salt_string";

            string expectedHash = CalculateExpectedHash(password, salt);

            var dbUser = new User
            {
                UserId = 1,
                Username = username,
                PasswordHash = expectedHash,
                Salt = salt,
                RoleId = 4
            };

            _userDalMock.Setup(d => d.GetUserByUsername(username)).Returns(dbUser);

            // Act
            var result = _sut.Login(username, password);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(username));
            Assert.That(result.UserId, Is.EqualTo(1));
        }

        [Test]
        public void Login_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            const string username = "user1";
            var dbUser = new User
            {
                Username = username,
                PasswordHash = "some_valid_hash",
                Salt = "some_salt"
            };

            _userDalMock.Setup(d => d.GetUserByUsername(username)).Returns(dbUser);

            // Act
            var result = _sut.Login(username, "wrong_password");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Login_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            const string username = "ghost_user";
            _userDalMock.Setup(d => d.GetUserByUsername(username)).Returns((User)null);

            // Act
            var result = _sut.Login(username, "any_password");

            // Assert
            Assert.That(result, Is.Null);
        }




        private string CalculateExpectedHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(combined);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}