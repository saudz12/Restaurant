using Restaurant.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Restaurant
{
    /// <summary>
    /// Interaction logic for OrderManagerWindow.xaml
    /// </summary>
    public partial class OrderManagerWindow : Window
    {
        private readonly OrderManagerViewModel _viewModel;

        public OrderManagerWindow(OrderManagerViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            Loaded += EmployeeOrderManagementWindow_Loaded;
        }

        private async void EmployeeOrderManagementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderManagerViewModel.DialogResult) && _viewModel.DialogResult.HasValue)
            {
                DialogResult = _viewModel.DialogResult;
            }
        }
    }
}
