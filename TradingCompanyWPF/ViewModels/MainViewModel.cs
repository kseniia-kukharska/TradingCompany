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
        private readonly IOrderManager _orderManager;
        private readonly IStatusDal _statusDal;

        private ObservableCollection<Order> _orders;
        private ObservableCollection<Status> _statuses;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private Status _selectedFilterStatus;
        private User _currentUser;

        public MainViewModel(IOrderManager orderManager, IStatusDal statusDal)
        {
            _orderManager = orderManager;
            _statusDal = statusDal;

            CurrentUser = App.CurrentUser;


            ApplyFilterCommand = new RelayCommand(o => ApplyFilter());
            ResetFilterCommand = new RelayCommand(o => ResetFilter());
            SaveChangesCommand = new RelayCommand(o => SaveChanges(), o => IsSeller);
            LogoutCommand = new RelayCommand(o => Logout(o));

            LoadData();
        }



        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSeller));
            }
        }

        public bool IsSeller => CurrentUser?.RoleId == 4;

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

                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            var filtered = _orderManager.GetFilteredOrders(StartDate, EndDate, SelectedFilterStatus?.StatusId);
            Orders = new ObservableCollection<Order>(filtered);
        }

        private void ResetFilter()
        {
            StartDate = null;
            EndDate = null;
            SelectedFilterStatus = null;

            ApplyFilter();
        }

        private void SaveChanges()
        {
            if (!IsSeller) return;

            try
            {
                int updatedCount = 0;
                foreach (var order in Orders)
                {
                    bool hasErrors = !string.IsNullOrEmpty(order[nameof(order.TotalAmount)]) ||
                                     !string.IsNullOrEmpty(order[nameof(order.OrderDate)]);

                    if (!hasErrors)
                    {                    
                        _orderManager.UpdateOrder(order, CurrentUser.UserId);
                        updatedCount++;
                    }
                }
                MessageBox.Show($"Successfully saved changes for {updatedCount} orders.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout(object window)
        {
            if (window is Window currentWindow)
            {
                var result = MessageBox.Show("Are you sure you want to log out?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
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