using Database.Services.Dtos;
using Database.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Restaurant.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;

        private string _nume;
        public string Nume
        {
            get => _nume;
            set
            {
                _nume = value;
                OnPropertyChanged();
                ValidateInput();
            }
        }

        private string _prenume;
        public string Prenume
        {
            get => _prenume;
            set
            {
                _prenume = value;
                OnPropertyChanged();
                ValidateInput();
            }
        }

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

        private string _telefon;
        public string Telefon
        {
            get => _telefon;
            set
            {
                _telefon = value;
                OnPropertyChanged();
            }
        }

        private string _adresa;
        public string Adresa
        {
            get => _adresa;
            set
            {
                _adresa = value;
                OnPropertyChanged();
            }
        }

        private bool _isAngajat;
        public bool IsAngajat
        {
            get => _isAngajat;
            set
            {
                _isAngajat = value;
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

        private bool _canRegister;
        public bool CanRegister
        {
            get => _canRegister;
            set
            {
                _canRegister = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;

            RegisterCommand = new RelayCommand(_ => Register(), _ => CanRegister);
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void ValidateInput()
        {
            CanRegister = !string.IsNullOrWhiteSpace(Nume) &&
                          !string.IsNullOrWhiteSpace(Prenume) &&
                          !string.IsNullOrWhiteSpace(Email) &&
                          !string.IsNullOrWhiteSpace(Password);
        }

        private async void Register()
        {
            ErrorMessage = string.Empty;

            try
            {
                var userDto = new UserRegisterDto
                {
                    Nume = Nume,
                    Prenume = Prenume,
                    Email = Email,
                    Password = Password,
                    Telefon = Telefon,
                    Adresa = Adresa,
                    IsAngajat = IsAngajat
                };

                await _userService.RegisterUserAsync(userDto);

                DialogResult = true;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
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
}