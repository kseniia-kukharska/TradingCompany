using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TradingCompany.WPF.Utilities;
using TradingCompanyDal.Interfaces;
using TradingCompanyDto;

namespace TradingCompany.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IProductDal _productDal;
        private ObservableCollection<Product> _products;
        private User _currentUser;

        public MainViewModel(IProductDal productDal)
        {
            _productDal = productDal;

            CurrentUser = App.CurrentUser;

            RefreshCommand = new RelayCommand(o => LoadData());

            LoadData();
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentUserName));
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        public string CurrentUserName => _currentUser?.Username ?? "Unknown";

        // Головна "фішка" безпеки UI: повертає True, тільки якщо це Адмін
        public bool IsAdmin => _currentUser?.Role?.RoleName == "Admin";

        // --- Команди (Commands) ---

        public ICommand RefreshCommand { get; }

        // --- Методи ---

        private void LoadData()
        {
            try
            {
                // Отримуємо список з бази
                var list = _productDal.GetAll();
                // Перетворюємо в ObservableCollection, щоб таблиця оновилась
                Products = new ObservableCollection<Product>(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}");
            }
        }

        // --- Реалізація INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}