using Database;
using Database.Enums;
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

        private ObservableCollection<FoodDisplayItem> _foodItems;
        public ObservableCollection<FoodDisplayItem> FoodItems
        {
            get => _foodItems;
            set { _foodItems = value; OnPropertyChanged(); }
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

        public ObservableCollection<CategoriiPreparate> AvailableCategorii { get; } = new ObservableCollection<CategoriiPreparate>(
            Enum.GetValues(typeof(CategoriiPreparate)).Cast<CategoriiPreparate>()
        );

        public ICommand RefreshCommand { get; }
        public ICommand AddNewPreparatCommand { get; }
        public ICommand AddNewMenuCommand { get; }

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

        public ICommand SearchCommand { get; }

        public FoodDisplayViewModel(
            IFoodDisplayService foodDisplayService,
            IMenuService menuService,
            IDataRefreshService refreshService)
        {
            _foodDisplayService = foodDisplayService;
            _menuService = menuService;
            _refreshService = refreshService;


            FoodItems = new ObservableCollection<FoodDisplayItem>();

            RefreshCommand = new RelayCommand(_ => LoadFoodItemsAsync().ConfigureAwait(false));
            AddNewPreparatCommand = new RelayCommand(_ => OpenAddPreparatWindow());
            SearchCommand = new RelayCommand(_ => FilterFoodItemsAsync().ConfigureAwait(false));
            AllergenFilters = new ObservableCollection<AllergenFilter>();
            AddNewMenuCommand = new RelayCommand(_ => OpenAddMenuWindow());

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
                // In a real application, you would log this error
                System.Diagnostics.Debug.WriteLine($"Error loading food items: {ex.Message}");
            }
        }

        private async Task LoadAllergensAsync()
        {
            try
            {
                // Assuming we add this to IFoodDisplayService
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
                // Get excluded allergens (the ones with IsExcluded = true)
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


        // Simple ICommand implementation for WPF
        private class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
            public void Execute(object parameter) => _execute(parameter);
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
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