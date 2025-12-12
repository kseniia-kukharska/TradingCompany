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

        private List<Order> _allOrders; // Cache for filtering
        private ObservableCollection<Order> _orders;
        private ObservableCollection<Status> _statuses;

        // Filter fields
        private DateTime? _startDate;
        private DateTime? _endDate;
        private Status _selectedFilterStatus;

        public MainViewModel(IOrderDal orderDal, IStatusDal statusDal)
        {
            _orderDal = orderDal;
            _statusDal = statusDal;
            CurrentUser = App.CurrentUser;

            // Commands
            RefreshCommand = new RelayCommand(o => LoadData());
            ApplyFilterCommand = new RelayCommand(o => ApplyFilter());
            ResetFilterCommand = new RelayCommand(o => ResetFilter());
            SaveChangesCommand = new RelayCommand(o => SaveChanges());
            LogoutCommand = new RelayCommand(o => Logout(o));

            LoadData();
        }

        // Properties
        public User CurrentUser { get; set; }
        public string CurrentUserName => CurrentUser?.Username ?? "Unknown";

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

        // Commands
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

                _allOrders = _orderDal.GetAll();
                Orders = new ObservableCollection<Order>(_allOrders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            if (_allOrders == null) return;

            var filtered = _allOrders.AsEnumerable();

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
            Orders = new ObservableCollection<Order>(_allOrders);
        }

        private void SaveChanges()
        {
            try
            {
                foreach (var order in Orders)
                {
                    _orderDal.Update(order);
                }
                MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout(object window)
        {
            if (window is Window currentWindow)
            {
                currentWindow.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}