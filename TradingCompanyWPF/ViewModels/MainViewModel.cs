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
        private readonly IOrderDal _orderDal;
        private readonly IStatusDal _statusDal;

        private ObservableCollection<Order> _orders;
        private ObservableCollection<Status> _statuses;
        private User _currentUser;

        private DateTime? _startDate;
        private DateTime? _endDate;
        private Status _selectedFilterStatus;

        public MainViewModel(IOrderDal orderDal, IStatusDal statusDal)
        {
            _orderDal = orderDal;
            _statusDal = statusDal;
            CurrentUser = App.CurrentUser;

            RefreshCommand = new RelayCommand(o => LoadData());
            ApplyFilterCommand = new RelayCommand(o => ApplyFilter());
            ResetFilterCommand = new RelayCommand(o => ResetFilter());
            SaveChangesCommand = new RelayCommand(o => SaveChanges());
            LogoutCommand = new RelayCommand(o => Logout(o));

            LoadData();
        }

        public User CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentUserName)); }
        }
        public string CurrentUserName => _currentUser?.Username ?? "Unknown";

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set { _orders = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Status> Statuses
        {
            get => _statuses;
            set { _statuses = value; OnPropertyChanged(); }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        public Status SelectedFilterStatus
        {
            get => _selectedFilterStatus;
            set { _selectedFilterStatus = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand LogoutCommand { get; }

        private void LoadData()
        {
            try
            {
                var statusList = _statusDal.GetAll();
                Statuses = new ObservableCollection<Status>(statusList);

                var orderList = _orderDal.GetAll();
                Orders = new ObservableCollection<Order>(orderList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveChanges()
        {
            try
            {
                foreach (var order in Orders)
                {
                    _orderDal.Update(order);
                }
                MessageBox.Show("Orders saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            var allOrders = _orderDal.GetAll();
            var filtered = allOrders.AsEnumerable();

            if (StartDate.HasValue)
                filtered = filtered.Where(o => o.OrderDate >= StartDate.Value);

            if (EndDate.HasValue)
                filtered = filtered.Where(o => o.OrderDate <= EndDate.Value);

            if (SelectedFilterStatus != null)
                filtered = filtered.Where(o => o.StatusId == SelectedFilterStatus.StatusId);

            Orders = new ObservableCollection<Order>(filtered.ToList());
        }

        private void ResetFilter()
        {
            StartDate = null;
            EndDate = null;
            SelectedFilterStatus = null;
            LoadData();
        }

        private void Logout(object window)
        {
            if (window is Window currentWindow)
            {
                var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    currentWindow.Close();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}