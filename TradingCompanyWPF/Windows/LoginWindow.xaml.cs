using System.Windows;
using TradingCompany.WPF.ViewModels;

namespace TradingCompany.WPF.Windows
{
    public partial class LoginWindow : Window
    {
        // Видаліть підписку на подію звідси. Вікно має бути "дурним" і просто відображати дані.
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;


            // ЦЕЙ БЛОК КОДУ ПОТРІБНО ВИДАЛИТИ АБО ЗАКОМЕНТУВАТИ:
            /* viewModel.LoginSuccess += (user) =>
            {
                this.DialogResult = true;
                this.Close();
            }; 
            */
        }
    }
}