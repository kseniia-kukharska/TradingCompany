using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TradingCompany.BL.Concrete;
using TradingCompany.BL.Interfaces;
using TradingCompany.WPF.ViewModels;
using TradingCompany.WPF.Windows;
using TradingCompanyDal.Concrete;
using TradingCompanyDal.Interfaces;
using TradingCompanyDto;

namespace TradingCompany.WPF
{
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            var loginViewModel = loginWindow.DataContext as LoginViewModel;

            if (loginViewModel == null)
            {
                loginViewModel = ServiceProvider.GetRequiredService<LoginViewModel>();
                loginWindow.DataContext = loginViewModel;
            }

            loginViewModel.LoginSuccess += (user) =>
            {
                App.CurrentUser = user;
                loginWindow.DialogResult = true;
                loginWindow.Close();
            };

            if (loginWindow.ShowDialog() == true)
            {
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

                // Shutdown application when main window closes
                mainWindow.Closed += (s, args) => Shutdown();

                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
             .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            //CreateDefaultAdmin(connectionString);

            // Register DALs
            services.AddTransient<IUserDal>(provider => new UserDal(connectionString));
            services.AddTransient<IProductDal>(provider => new ProductDal(connectionString));

            // NEW: Add Order and Status DALs
            services.AddTransient<IOrderDal>(provider => new OrderDal(connectionString));
            services.AddTransient<IStatusDal>(provider => new StatusDal(connectionString));
            services.AddTransient<ICustomerDal>(provider => new CustomerDal(connectionString));

            // Business Logic
            services.AddTransient<IAuthManager, AuthManager>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();

            // Windows
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
        }


        // Приклад коду для C# (Seed Data)
        private static void CreateDefaultAdmin(string connectionString)
        {
            var userDal = new UserDal(connectionString);
            var authManager = new AuthManager(userDal); // Припускаємо, що у вас є AuthManager

            // Перевіряємо, чи існує адмін
            var existingUser = userDal.GetUserByUsername("seller");
            if (existingUser == null)
            {
                // Реєстрація через AuthManager автоматично згенерує правильний Hash та Salt
                bool result = authManager.Register("seller", "seller123", 4); // Логін: admin, Пароль: admin123

                if (result)
                {
                    System.Console.WriteLine("Default Admin created successfully!");
                }
            }
        }
    }
}