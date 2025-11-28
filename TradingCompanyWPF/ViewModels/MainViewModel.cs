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

        public bool IsAdmin => _currentUser?.Role?.RoleName == "Admin";

        public ICommand RefreshCommand { get; }


        private void LoadData()
        {
            try
            {
                var list = _productDal.GetAll();
                Products = new ObservableCollection<Product>(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Data loading error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}