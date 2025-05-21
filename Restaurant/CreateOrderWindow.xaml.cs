using Database.Services.Dtos;
using Restaurant.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;

namespace Restaurant
{
    public partial class CreateOrderWindow : Window
    {
        private readonly CreateOrderViewModel _viewModel;

        public CreateOrderWindow(CreateOrderViewModel viewModel, FoodDisplayItem initialItem = null)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.Initialize(initialItem);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CreateOrderViewModel.DialogResult) && _viewModel.DialogResult.HasValue)
            {
                DialogResult = _viewModel.DialogResult;
            }
        }
    }
}