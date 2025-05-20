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
    /// Interaction logic for AddMenuWindow.xaml
    /// </summary>
    public partial class AddMenuWindow : Window
    {
        private readonly AddMenuViewModel _viewModel;

        public AddMenuWindow(AddMenuViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            // Handle dialog closing via ViewModel
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AddMenuViewModel.DialogResult) && _viewModel.DialogResult.HasValue)
            {
                DialogResult = _viewModel.DialogResult;
            }
        }
    }
}
