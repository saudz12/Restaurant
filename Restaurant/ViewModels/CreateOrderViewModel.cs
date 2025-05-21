using Database;
using Database.Enums;
using Database.Services.Dtos;
using Database.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Restaurant.ViewModels
{
    public class CreateOrderViewModel : INotifyPropertyChanged
    {
        private readonly IComandaService _comandaService;
        private readonly IUserStateService _userStateService;
        private readonly IDataRefreshService _refreshService;
        public ObservableCollection<OrderItemViewModel> CartItems { get; } = new ObservableCollection<OrderItemViewModel>();

        private FoodDisplayItem _initialFoodItem;

        private double _subtotal;
        public double Subtotal
        {
            get => _subtotal;
            set
            {
                _subtotal = value;
                OnPropertyChanged();
                CalculateTotalPrice();
            }
        }

        private double _discountAmount;
        public double DiscountAmount
        {
            get => _discountAmount;
            set
            {
                _discountAmount = value;
                OnPropertyChanged();
                CalculateTotalPrice();
            }
        }

        private double _deliveryFee;
        public double DeliveryFee
        {
            get => _deliveryFee;
            set
            {
                _deliveryFee = value;
                OnPropertyChanged();
                CalculateTotalPrice();
            }
        }

        private double _totalPrice;
        public double TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged();
            }
        }

        private string _estimatedDeliveryTime;
        public string EstimatedDeliveryTime
        {
            get => _estimatedDeliveryTime;
            set
            {
                _estimatedDeliveryTime = value;
                OnPropertyChanged();
            }
        }

        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand CancelCommand { get; }

        private bool _canPlaceOrder;
        public bool CanPlaceOrder
        {
            get => _canPlaceOrder;
            set
            {
                _canPlaceOrder = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public string DeliveryAddress => _userStateService.CurrentUser?.Adresa ?? string.Empty;
        public string UserPhone => _userStateService.CurrentUser?.Telefon ?? string.Empty;

        public CreateOrderViewModel(
            IComandaService comandaService,
            IUserStateService userStateService,
            IDataRefreshService refreshService)
        {
            _comandaService = comandaService;
            _userStateService = userStateService;
            _refreshService = refreshService;

            IncreaseQuantityCommand = new RelayCommand(item => IncreaseQuantity((OrderItemViewModel)item));
            DecreaseQuantityCommand = new RelayCommand(item => DecreaseQuantity((OrderItemViewModel)item));
            RemoveItemCommand = new RelayCommand(item => RemoveItem((OrderItemViewModel)item));
            PlaceOrderCommand = new RelayCommand(_ => PlaceOrder(), _ => CanPlaceOrder);
            CancelCommand = new RelayCommand(_ => Cancel());

            EstimatedDeliveryTime = DateTime.Now.AddHours(1).ToString("HH:mm");

            DeliveryFee = 15.0; 
        }

        public void Initialize(FoodDisplayItem initialItem = null)
        {
            if (initialItem != null)
            {
                _initialFoodItem = initialItem;
                AddItemToCart(initialItem);
            }

            UpdateOrderStatus();
        }

        public void InitializeWithCartItems(List<OrderItemViewModel> cartItems)
        {
            CartItems.Clear();

            foreach (var item in cartItems)
            {
                CartItems.Add(item);
            }

            CalculateSubtotal();
            UpdateOrderStatus();
        }

        private void AddItemToCart(FoodDisplayItem item)
        {
            var existing = CartItems.FirstOrDefault(i => i.ItemId == item.Id && i.ItemType == item.Tip);

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                var newItem = new OrderItemViewModel
                {
                    ItemId = item.Id,
                    ItemName = item.Nume,
                    ItemType = item.Tip,
                    UnitPrice = item.Pret,
                    Quantity = 1
                };

                newItem.PropertyChanged += CartItem_PropertyChanged;
                CartItems.Add(newItem);
            }

            CalculateSubtotal();
            UpdateOrderStatus();
        }

        private void IncreaseQuantity(OrderItemViewModel item)
        {
            item.Quantity++;
            CalculateSubtotal();
            UpdateOrderStatus();
        }

        private void DecreaseQuantity(OrderItemViewModel item)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
                CalculateSubtotal();
                UpdateOrderStatus();
            }
        }

        private void RemoveItem(OrderItemViewModel item)
        {
            item.PropertyChanged -= CartItem_PropertyChanged;
            CartItems.Remove(item);
            CalculateSubtotal();
            UpdateOrderStatus();
        }

        private void CartItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderItemViewModel.Quantity) ||
                e.PropertyName == nameof(OrderItemViewModel.TotalPrice))
            {
                CalculateSubtotal();
            }
        }

        private void CalculateSubtotal()
        {
            // Free delivery after 50, 10% discount adter 100
            Subtotal = CartItems.Sum(item => item.TotalPrice);

            if (Subtotal >= 100)
            {
                DiscountAmount = Subtotal * 0.1;
            }
            else
            {
                DiscountAmount = 0;
            }

            if (Subtotal >= 50)
            {
                DeliveryFee = 0;
            }
            else
            {
                DeliveryFee = 15; 
            }
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = Subtotal - DiscountAmount + DeliveryFee;
        }

        private void UpdateOrderStatus()
        {
            CanPlaceOrder = CartItems.Count > 0;

            ErrorMessage = string.Empty;
        }

        private async void PlaceOrder()
        {
            if (!CartItems.Any())
            {
                ErrorMessage = "Your cart is empty. Please add items to your order.";
                return;
            }

            try
            {
                var orderDto = new ComandaCreateDto
                {
                    UserEmail = _userStateService.CurrentUserEmail,
                    Items = CartItems.Select(item => new ComandaItemCreateDto
                    {
                        PreparatId = item.ItemId,
                        Cantitate = item.Quantity
                    }).ToList()
                };

                var result = await _comandaService.CreateComandaAsync(orderDto);

                _refreshService.NotifyDataChanged();

                DialogResult = true;

                MessageBox.Show(
                    $"Order placed successfully!\nOrder #: {result.Id}\nEstimated delivery: {EstimatedDeliveryTime}",
                    "Order Placed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to place order: {ex.Message}";
            }
        }

        private void Cancel()
        {
            DialogResult = false;
        }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class OrderItemViewModel : INotifyPropertyChanged
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; } 

        private double _unitPrice;
        public double UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public double TotalPrice => UnitPrice * Quantity;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}