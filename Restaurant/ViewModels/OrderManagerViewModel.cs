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
using System.Windows;

namespace Restaurant.ViewModels;

public class OrderManagerViewModel : INotifyPropertyChanged
{
    private readonly IComandaService _comandaService;
    private readonly IDataRefreshService _refreshService;

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
            OnPropertyChanged(nameof(CanCancelOrder));

            if (_selectedOrder != null)
            {
                SelectedStatus = _selectedOrder.StareComanda;
            }
        }
    }

    private StareComanda _selectedStatus;
    public StareComanda SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanUpdateStatus));
        }
    }

    public Array AvailableStatuses => Enum.GetValues(typeof(StareComanda));

    public bool IsOrderSelected => SelectedOrder != null;
    public bool CanCancelOrder => SelectedOrder != null &&
                                 (SelectedOrder.StareComanda == StareComanda.Inregistrata ||
                                  SelectedOrder.StareComanda == StareComanda.InPregatire);

    public bool CanUpdateStatus => SelectedOrder != null && SelectedStatus != SelectedOrder.StareComanda;

    // Commands
    public ICommand RefreshCommand { get; }
    public ICommand UpdateStatusCommand { get; }
    public ICommand CancelOrderCommand { get; }
    public ICommand CloseCommand { get; }

    public OrderManagerViewModel(
        IComandaService comandaService,
        IDataRefreshService refreshService)
    {
        _comandaService = comandaService;
        _refreshService = refreshService;

        RefreshCommand = new RelayCommand(_ => LoadOrdersAsync());
        UpdateStatusCommand = new RelayCommand(_ => UpdateOrderStatusAsync(), _ => CanUpdateStatus);
        CancelOrderCommand = new RelayCommand(_ => CancelOrderAsync(), _ => CanCancelOrder);
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

            // Clear existing orders
            Orders.Clear();

            // Load all orders (for employees)
            var orders = await _comandaService.GetAllComenziAsync();

            // Sort by date, newest first
            var sortedOrders = orders.OrderByDescending(o => o.DataCreare);

            foreach (var order in sortedOrders)
            {
                Orders.Add(order);
            }

            if (Orders.Count == 0)
            {
                ErrorMessage = "No orders found in the system.";
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

    private async Task UpdateOrderStatusAsync()
    {
        if (SelectedOrder == null || !CanUpdateStatus)
            return;

        try
        {
            IsLoading = true;

            // Create update DTO
            var updateDto = new ComandaUpdateStareDto
            {
                ComandaId = SelectedOrder.Id,
                StareComanda = SelectedStatus
            };

            // Update the order status
            await _comandaService.UpdateComandaStareAsync(updateDto);

            // Update the local order object
            SelectedOrder.StareComanda = SelectedStatus;

            // Notify UI
            OnPropertyChanged(nameof(SelectedOrder));

            // Notify data refresh service
            _refreshService.NotifyDataChanged();

            // Show success message
            MessageBox.Show(
                $"Order #{SelectedOrder.Id} status updated to: {GetStatusDescription(SelectedStatus)}",
                "Status Updated",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Refresh the list
            await LoadOrdersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating order status: {ex.Message}";
            MessageBox.Show(
                $"Failed to update order status: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CancelOrderAsync()
    {
        if (SelectedOrder == null || !CanCancelOrder)
            return;

        // Confirm cancellation
        var result = MessageBox.Show(
            $"Are you sure you want to cancel Order #{SelectedOrder.Id}?\n\nThis action cannot be undone.",
            "Confirm Cancellation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            IsLoading = true;

            await _comandaService.DeleteComandaAsync(SelectedOrder.Id);

            _refreshService.NotifyDataChanged();

            MessageBox.Show(
                $"Order #{SelectedOrder.Id} has been cancelled and removed.",
                "Order Cancelled",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            SelectedOrder = null;

            await LoadOrdersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error cancelling order: {ex.Message}";
            MessageBox.Show(
                $"Failed to cancel order: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
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
