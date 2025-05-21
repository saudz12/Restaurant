using Database;
using Database.Enums;
using Database.Services;
using Database.Services.Dtos;
using Database.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Restaurant.ViewModels
{
    public class FoodDisplayViewModel : INotifyPropertyChanged
    {
        private readonly IFoodDisplayService _foodDisplayService;
        private readonly IMenuService _menuService;
        private readonly IDataRefreshService _refreshService;
        private readonly IUserStateService _userStateService;

        private ObservableCollection<FoodDisplayItem> _foodItems;
        public ObservableCollection<FoodDisplayItem> FoodItems
        {
            get => _foodItems;
            set { _foodItems = value; OnPropertyChanged(); }
        }
        public ObservableCollection<CategoriiPreparate> AvailableCategorii { get; } = new ObservableCollection<CategoriiPreparate>(
            Enum.GetValues(typeof(CategoriiPreparate)).Cast<CategoriiPreparate>()
        );

        private ObservableCollection<OrderItemViewModel> _cartItems = new ObservableCollection<OrderItemViewModel>();
        public ObservableCollection<OrderItemViewModel> CartItems => _cartItems;

        private ObservableCollection<AllergenFilter> _allergenFilters;
        public ObservableCollection<AllergenFilter> AllergenFilters
        {
            get => _allergenFilters;
            set
            {
                _allergenFilters = value;
                OnPropertyChanged();
            }
        }

        private FoodDisplayItem _selectedFoodItem;
        public FoodDisplayItem SelectedFoodItem
        {
            get => _selectedFoodItem;
            set
            {
                _selectedFoodItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsItemSelected));
                OnPropertyChanged(nameof(HasAlergeni));
            }
        }

        public bool IsItemSelected => SelectedFoodItem != null;
        public bool HasAlergeni => SelectedFoodItem?.Alergeni?.Any() ?? false;
        public int CartItemCount => _cartItems.Sum(item => item.Quantity);
        public bool IsLoggedIn => _userStateService.IsLoggedIn;
        public bool IsEmployee => _userStateService.IsEmployee;
        public bool IsCustomer => IsLoggedIn && !IsEmployee;
        public string CurrentUserName => IsLoggedIn ? $"{_userStateService.CurrentUser.Nume} {_userStateService.CurrentUser.Prenume}" : string.Empty;

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchCommand.Execute(null);
            }
        }

        private CategoriiPreparate? _selectedCategorie;
        public CategoriiPreparate? SelectedCategorie
        {
            get => _selectedCategorie;
            set
            {
                _selectedCategorie = value;
                OnPropertyChanged();
                LoadFoodItemsAsync().ConfigureAwait(false);
            }
        }


        public ICommand RefreshCommand { get; }
        public ICommand AddNewPreparatCommand { get; }
        public ICommand AddNewMenuCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand CreateOrderCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand ViewOrdersCommand { get; }
        public ICommand ManageOrdersCommand { get; }

        public FoodDisplayViewModel(
            IFoodDisplayService foodDisplayService,
            IMenuService menuService,
            IDataRefreshService refreshService,
            IUserStateService userStateService)
        {
            _foodDisplayService = foodDisplayService;
            _menuService = menuService;
            _refreshService = refreshService;
            _userStateService = userStateService;

            FoodItems = new ObservableCollection<FoodDisplayItem>();

            RefreshCommand = new RelayCommand(_ => LoadFoodItemsAsync().ConfigureAwait(false));
            AddNewPreparatCommand = new RelayCommand(_ => OpenAddPreparatWindow());
            SearchCommand = new RelayCommand(_ => FilterFoodItemsAsync().ConfigureAwait(false));
            AllergenFilters = new ObservableCollection<AllergenFilter>();
            AddNewMenuCommand = new RelayCommand(_ => OpenAddMenuWindow());

            LoginCommand = new RelayCommand(_ => OpenLoginWindow());
            RegisterCommand = new RelayCommand(_ => OpenRegisterWindow());
            LogoutCommand = new RelayCommand(_ => Logout(), _ => IsLoggedIn);
            ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
            CreateOrderCommand = new RelayCommand(_ => OpenCreateOrderWindow(), _ => IsCustomer);
            AddToCartCommand = new RelayCommand(_ => AddToCart(), _ => SelectedFoodItem != null && SelectedFoodItem.IsAvailable);
            ViewOrdersCommand = new RelayCommand(_ => OpenOrderHistoryWindow(), _ => IsCustomer);
            ManageOrdersCommand = new RelayCommand(_ => OpenOrderManagementWindow(), _ => IsEmployee);

            // Subscribe to data change notifications
            _refreshService.DataChanged += async (s, e) => await LoadFoodItemsAsync();
        }

        public async Task InitializeAsync()
        {
            await LoadAllergensAsync();
            await LoadFoodItemsAsync();
        }

        private async Task LoadFoodItemsAsync()
        {
            FoodItems.Clear();

            try
            {
                var items = _selectedCategorie.HasValue
                    ? await _foodDisplayService.GetFoodItemsByCategorieAsync(_selectedCategorie.Value)
                    : await _foodDisplayService.GetAllFoodItemsAsync();

                foreach (var item in items)
                {
                    FoodItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading food items: {ex.Message}");
            }
        }

        private async Task LoadAllergensAsync()
        {
            try
            {
                var allergens = await _foodDisplayService.GetAllAllergensAsync();

                AllergenFilters.Clear();
                foreach (var allergen in allergens)
                {
                    AllergenFilters.Add(new AllergenFilter
                    {
                        Name = allergen,
                        IsExcluded = false
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading allergens: {ex.Message}");
            }
        }

        private async Task FilterFoodItemsAsync()
        {
            try
            {
                var excludedAllergens = AllergenFilters
                    .Where(af => af.IsExcluded)
                    .Select(af => af.Name)
                    .ToList();

                var filteredItems = await _foodDisplayService.SearchFoodItemsAsync(
                    SearchText,
                    excludedAllergens,
                    _selectedCategorie);

                FoodItems.Clear();
                foreach (var item in filteredItems)
                {
                    FoodItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering food items: {ex.Message}");
            }
        }

        private void OpenAddPreparatWindow()
        {
            try
            {
                var viewModel = App.ServiceProvider.GetRequiredService<AddPreparatViewModel>();

                var window = new AddPreparatWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                bool? result = window.ShowDialog();

                if (result == true)
                {
                    LoadFoodItemsAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Add Dish window: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OpenAddMenuWindow()
        {
            try
            {
                var viewModel = App.ServiceProvider.GetRequiredService<AddMenuViewModel>();

                var window = new AddMenuWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                bool? result = window.ShowDialog();

                if (result == true)
                {
                    LoadFoodItemsAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Add Menu window: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void UserStateService_UserStateChanged(object sender, EventArgs e)
        {
            // Notify UI of property changes related to user state
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsEmployee));
            OnPropertyChanged(nameof(IsCustomer));
            OnPropertyChanged(nameof(CurrentUserName));
        }

        private void OpenLoginWindow()
        {
            try
            {
                var viewModel = App.ServiceProvider.GetRequiredService<LoginViewModel>();
                var window = new LoginWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                bool? result = window.ShowDialog();

                // Force UI update after login window closes
                if (result == true)
                {
                    // Manually trigger property change notifications for all user state properties
                    OnPropertyChanged(nameof(IsLoggedIn));
                    OnPropertyChanged(nameof(IsEmployee));
                    OnPropertyChanged(nameof(IsCustomer));
                    OnPropertyChanged(nameof(CurrentUserName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Login window: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OpenRegisterWindow()
        {
            try
            {
                var viewModel = App.ServiceProvider.GetRequiredService<RegisterViewModel>();
                var window = new RegisterWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Register window: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Logout()
        {
            try
            {
                _userStateService.Logout();

                // Force UI update after logout
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(IsEmployee));
                OnPropertyChanged(nameof(IsCustomer));
                OnPropertyChanged(nameof(CurrentUserName));

                // Debug message
                System.Diagnostics.Debug.WriteLine("User logged out successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error during logout: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OpenCreateOrderWindow()
        {
            try
            {
                if (!IsCustomer)
                {
                    MessageBox.Show(
                        "You need to be logged in as a customer to place an order.",
                        "Login Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (_cartItems.Count == 0)
                {
                    MessageBox.Show(
                        "Your order is empty. Please add items to your order first.",
                        "Empty Order",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var viewModel = App.ServiceProvider.GetRequiredService<CreateOrderViewModel>();

                var window = new CreateOrderWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                viewModel.InitializeWithCartItems(_cartItems.ToList());

                bool? result = window.ShowDialog();

                if (result == true)
                {
                    _cartItems.Clear();
                    OnPropertyChanged(nameof(CartItemCount));
                    LoadFoodItemsAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening order window: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AddToCart()
        {
            if (SelectedFoodItem == null) return;

            var existingItem = _cartItems.FirstOrDefault(i =>
                i.ItemId == SelectedFoodItem.Id && i.ItemType == SelectedFoodItem.Tip);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var newItem = new OrderItemViewModel
                {
                    ItemId = SelectedFoodItem.Id,
                    ItemName = SelectedFoodItem.Nume,
                    ItemType = SelectedFoodItem.Tip,
                    UnitPrice = SelectedFoodItem.Pret,
                    Quantity = 1
                };

                _cartItems.Add(newItem);
            }

            OnPropertyChanged(nameof(CartItemCount));

            MessageBox.Show(
                $"{SelectedFoodItem.Nume} added to your order.",
                "Added to Order",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void OpenOrderHistoryWindow()
        {
            try
            {
                if (!IsCustomer)
                {
                    MessageBox.Show(
                        "You need to be logged in as a customer to view your orders.",
                        "Login Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var viewModel = App.ServiceProvider.GetRequiredService<OrderHistoryViewModel>();

                var window = new OrderHistoryWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening order history: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OpenOrderManagementWindow()
        {
            try
            {
                if (!IsEmployee)
                {
                    MessageBox.Show(
                        "You need to be logged in as an employee to manage orders.",
                        "Employee Access Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var viewModel = App.ServiceProvider.GetRequiredService<OrderManagerViewModel>();

                // Create window
                var window = new OrderManagerWindow(viewModel)
                {
                    Owner = Application.Current.MainWindow
                };

                // Show as dialog
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening order management: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AllergenFilter : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private bool _isExcluded;
        public bool IsExcluded
        {
            get => _isExcluded;
            set
            {
                _isExcluded = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}