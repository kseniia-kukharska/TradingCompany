using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TradingCompany.BL.Interfaces;
using TradingCompany.WPF.Utilities;
using TradingCompanyDto;

namespace TradingCompany.WPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthManager _authManager;
        private string _username;

        public event Action<User> LoginSuccess;

        public LoginViewModel(IAuthManager authManager)
        {
            _authManager = authManager;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        private void ExecuteLogin(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            try
            {
                var user = _authManager.Login(Username, password);

                if (user != null)
                {
                    LoginSuccess?.Invoke(user);
                }
                else
                {
                    MessageBox.Show("Невірний логін або пароль.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка: {ex.Message}");
            }
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}