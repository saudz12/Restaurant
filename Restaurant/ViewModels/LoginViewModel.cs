using Database.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Restaurant.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IUserStateService _userStateService;

    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
            ValidateInput();
        }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
            ValidateInput();
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

    private bool _canLogin;
    public bool CanLogin
    {
        get => _canLogin;
        set
        {
            _canLogin = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoginCommand { get; }
    public ICommand CancelCommand { get; }

    public LoginViewModel(IUserStateService userStateService)
    {
        _userStateService = userStateService;

        LoginCommand = new RelayCommand(_ => Login(), _ => CanLogin);
        CancelCommand = new RelayCommand(_ => Cancel());
    }

    private void ValidateInput()
    {
        CanLogin = !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
    }

    private async void Login()
    {
        ErrorMessage = string.Empty;

        try
        {
            bool success = await _userStateService.LoginAsync(Email, Password);

            if (success)
            {
                DialogResult = true;
            }
            else
            {
                ErrorMessage = "Invalid email or password. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
    }

    private void Cancel()
    {
        DialogResult = false;
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

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
