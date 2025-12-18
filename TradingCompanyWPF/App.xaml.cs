using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TradingCompanyBL.Concrete;
using TradingCompanyBL.Interfaces;
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

            // DAL
            services.AddTransient<IUserDal>(p => new UserDal(connectionString));
            services.AddTransient<IOrderDal>(p => new OrderDal(connectionString));
            services.AddTransient<IStatusDal>(p => new StatusDal(connectionString));

            // BLL
            services.AddTransient<IAuthManager, AuthManager>();
            services.AddTransient<IOrderManager, OrderManager>(); // Додано менеджер

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