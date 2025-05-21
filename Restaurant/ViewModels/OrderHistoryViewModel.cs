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
using System.Windows.Input;

namespace Restaurant.ViewModels;

public class OrderHistoryViewModel : INotifyPropertyChanged
{
    private readonly IComandaService _comandaService;
    private readonly IUserStateService _userStateService;

    public ObservableCollection<ComandaDto> Orders { get; } = new ObservableCollection<ComandaDto>();

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
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

    private ComandaDto _selectedOrder;
    public ComandaDto SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsOrderSelected));
        }
    }

    public bool IsOrderSelected => SelectedOrder != null;

    public ICommand RefreshCommand { get; }
    public ICommand CloseCommand { get; }

    public OrderHistoryViewModel(IComandaService comandaService, IUserStateService userStateService)
    {
        _comandaService = comandaService;
        _userStateService = userStateService;

        RefreshCommand = new RelayCommand(_ => LoadOrdersAsync());
        CloseCommand = new RelayCommand(_ => DialogResult = true);
    }

    public async Task InitializeAsync()
    {
        await LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            Orders.Clear();

            string userEmail = _userStateService.CurrentUserEmail;

            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorMessage = "No user is currently logged in.";
                return;
            }

            var orders = await _comandaService.GetComenziByUserAsync(userEmail);

            var sortedOrders = orders.OrderByDescending(o => o.DataCreare);

            foreach (var order in sortedOrders)
            {
                Orders.Add(order);
            }

            if (Orders.Count == 0)
            {
                ErrorMessage = "You have no orders yet.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading orders: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public static string GetStatusDescription(StareComanda status)
    {
        switch (status)
        {
            case StareComanda.Inregistrata:
                return "Registered";
            case StareComanda.InPregatire:
                return "Being Prepared";
            case StareComanda.PeDrum:
                return "On the Way";
            case StareComanda.Livrata:
                return "Delivered";
            case StareComanda.Anulata:
                return "Canceled";
            default:
                return status.ToString();
        }
    }

    // Property for dialog result
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