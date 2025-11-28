using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
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
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();

            // Правильний спосіб отримати ViewModel
            var loginViewModel = loginWindow.DataContext as LoginViewModel;

            // Якщо раптом DataContext порожній (хоча конструктор вікна має його задати)
            if (loginViewModel == null)
            {
                loginViewModel = ServiceProvider.GetRequiredService<LoginViewModel>();
                loginWindow.DataContext = loginViewModel;
            }

            // Тепер підписка точно спрацює
            loginViewModel.LoginSuccess += (user) =>
            {
                App.CurrentUser = user;
                loginWindow.DialogResult = true;
                loginWindow.Close();
            };

            if (loginWindow.ShowDialog() == true)
            {
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
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
             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // <-- Змінено тут
             .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            // --- РЕЄСТРАЦІЯ DAL (База даних) ---
            // Ви вже зареєстрували UserDal, але забули ProductDal
            services.AddTransient<IUserDal>(provider => new UserDal(connectionString));
            services.AddTransient<IProductDal>(provider => new ProductDal(connectionString)); // <--- ДОДАНО

            // Якщо у вас є інші DAL (CustomerDal, OrderDal), додайте їх сюди ж аналогічно:
            // services.AddTransient<ICustomerDal>(provider => new CustomerDal(connectionString));

            // --- РЕЄСТРАЦІЯ BL (Бізнес-логіка) ---
            services.AddTransient<IAuthManager, AuthManager>();

            // --- РЕЄСТРАЦІЯ ViewModels ---
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>(); // <--- ДОДАНО (Важливо для MainWindow)

            // --- РЕЄСТРАЦІЯ Вікон ---
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
        }
    }
}