using Database.Enums;
using Database.Services.Dtos;
using Database.Services.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Restaurant.ViewModels
{
    public class AddMenuViewModel : INotifyPropertyChanged
    {
        private readonly IMenuService _menuService;
        private readonly IPreparatService _preparatService;
        private readonly IDataRefreshService _refreshService;

        private string _menuName;
        public string MenuName
        {
            get => _menuName;
            set
            {
                _menuName = value;
                OnPropertyChanged();
                ValidateCanSave();
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private CategoriiPreparate _selectedCategory;
        public CategoriiPreparate SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
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

        private double _discountPercentage;
        public double DiscountPercentage
        {
            get => _discountPercentage;
            set
            {
                _discountPercentage = value;
                OnPropertyChanged();
            }
        }

        private double _finalPrice;
        public double FinalPrice
        {
            get => _finalPrice;
            set
            {
                _finalPrice = value;
                OnPropertyChanged();
            }
        }

        private int _baseQuantity;
        public int BaseQuantity
        {
            get => _baseQuantity;
            set
            {
                _baseQuantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private bool _canSave;
        public bool CanSave
        {
            get => _canSave;
            set
            {
                _canSave = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoriiPreparate> AvailableCategories { get; } = new ObservableCollection<CategoriiPreparate>(
            Enum.GetValues(typeof(CategoriiPreparate)).Cast<CategoriiPreparate>()
        );

        public ObservableCollection<PreparatDto> AvailablePreparate { get; } = new ObservableCollection<PreparatDto>();

        public ObservableCollection<MenuItemViewModel> SelectedItems { get; } = new ObservableCollection<MenuItemViewModel>();

        // Commands
        public ICommand BrowseImageCommand { get; }
        public ICommand AddPreparatCommand { get; }
        public ICommand RemovePreparatCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddMenuViewModel(
            IMenuService menuService,
            IPreparatService preparatService,
            IDataRefreshService refreshService)
        {
            _menuService = menuService;
            _preparatService = preparatService;
            _refreshService = refreshService;

            SelectedCategory = CategoriiPreparate.MicDejun; 
            DiscountPercentage = _menuService.GetMenuDiscount() * 100; // Convert from decimal to percentage

            BrowseImageCommand = new RelayCommand(_ => BrowseImage());
            AddPreparatCommand = new RelayCommand(p => AddPreparatToMenu((PreparatDto)p));
            RemovePreparatCommand = new RelayCommand(p => RemovePreparatFromMenu((MenuItemViewModel)p));
            SaveCommand = new RelayCommand(_ => SaveMenu(), _ => CanSave);
            CancelCommand = new RelayCommand(_ => Cancel());

            LoadPreparateAsync();
        }

        private async void LoadPreparateAsync()
        {
            try
            {
                var preparate = await _preparatService.GetAllPreparateAsync();

                AvailablePreparate.Clear();
                foreach (var preparat in preparate)
                {
                    AvailablePreparate.Add(preparat);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error loading dishes: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void BrowseImage()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Title = "Select menu image"
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePath = dialog.FileName;
            }
        }

        private void AddPreparatToMenu(PreparatDto preparat)
        {
            if (preparat == null) return;

            var existingItem = SelectedItems.FirstOrDefault(i => i.PreparatId == preparat.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += 100; 
            }
            else
            {
                // Add new item to the list
                var menuItem = new MenuItemViewModel
                {
                    PreparatId = preparat.Id,
                    PreparatName = preparat.Nume,
                    BaseQuantity = preparat.CantitatePortie, 
                    Quantity = 100, 
                    UnitPrice = preparat.Pret
                };

                menuItem.PropertyChanged += MenuItem_PropertyChanged;
                SelectedItems.Add(menuItem);
            }

            CalculateTotalPrice();
            ValidateCanSave();
        }

        private void RemovePreparatFromMenu(MenuItemViewModel item)
        {
            if (item == null) return;

            SelectedItems.Remove(item);
            CalculateTotalPrice();
            ValidateCanSave();
        }

        private void MenuItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MenuItemViewModel.Quantity))
            {
                CalculateTotalPrice();
            }
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = SelectedItems.Sum(item => item.TotalPrice);

            FinalPrice = TotalPrice * (1 - (DiscountPercentage / 100));
        }

        private void ValidateCanSave()
        {
            CanSave = !string.IsNullOrWhiteSpace(MenuName) && SelectedItems.Count > 0;
        }

        private async void SaveMenu()
        {
            try
            {
                var menuDto = new MenuCreateDto
                {
                    Nume = MenuName,
                    PozaUrl = ImagePath,
                    Categorie = SelectedCategory,
                    ListaPreparate = SelectedItems.Select(item => new MenuPreparatCreateDto
                    {
                        PreparatId = item.PreparatId,
                        Cantitate = item.Quantity
                    }).ToList()
                };

                await _menuService.CreateMeniuAsync(menuDto);

                _refreshService.NotifyDataChanged();

                DialogResult = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error saving menu: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
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
    }

    public class MenuItemViewModel : INotifyPropertyChanged
    {
        public int PreparatId { get; set; }

        private string _preparatName;
        public string PreparatName
        {
            get => _preparatName;
            set
            {
                _preparatName = value;
                OnPropertyChanged();
            }
        }

        public int BaseQuantity { get; set; }

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

        public double TotalPrice => UnitPrice * ((double)Quantity / BaseQuantity);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
