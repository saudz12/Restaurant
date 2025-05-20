using Restaurant.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddPreparatWindow.xaml
    /// </summary>
    public partial class AddPreparatWindow : Window
    {
        public AddPreparatWindow(AddPreparatViewModel viewModel)
        {
            InitializeComponent();

            // Set the DataContext to the ViewModel
            DataContext = viewModel;

            // Subscribe to close requests
            viewModel.CloseRequested += (sender, shouldSave) =>
            {
                DialogResult = shouldSave;
                Close();
            };
        }
    }
}
