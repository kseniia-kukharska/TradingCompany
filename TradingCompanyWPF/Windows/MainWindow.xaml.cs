using System.Windows;
using TradingCompany.WPF.ViewModels;

namespace TradingCompany.WPF.Windows
{
    public partial class MainWindow : Window
    {

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}