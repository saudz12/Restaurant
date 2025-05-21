using Restaurant.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restaurant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FoodDisplayViewModel _viewModel;

        public MainWindow(FoodDisplayViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            _viewModel = viewModel;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private void AllergenFilter_Changed(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as FoodDisplayViewModel;
            if (viewModel?.SearchCommand != null)
            {
                viewModel.SearchCommand.Execute(null);
            }
        }

    }
}