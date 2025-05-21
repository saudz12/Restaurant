using Database.Models;
using Database.Repositories.Interfaces;
using Database.Services.Dtos;
using Database.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return null;

        return MapToUserDto(user);
    }

    public async Task<UserDto> RegisterUserAsync(UserRegisterDto userDto)
    {
        if (await _userRepository.ExistsAsync(userDto.Email))
            throw new InvalidOperationException($"User with email {userDto.Email} already exists");

        var user = new User
        {
            Email = userDto.Email,
            Password = userDto.Password,
            Nume = userDto.Nume,
            Prenume = userDto.Prenume,
            Adresa = userDto.Adresa,
            Telefon = userDto.Telefon,
            Angajat = userDto.IsAngajat
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToUserDto(createdUser);
    }

    public async Task<UserDto> UpdateUserAsync(UserUpdateDto userDto)
    {
        var user = await _userRepository.GetByEmailAsync(userDto.Email);
        if (user == null)
            throw new KeyNotFoundException($"User with email {userDto.Email} not found");

        user.Nume = userDto.Nume;
        user.Prenume = userDto.Prenume;
        user.Adresa = userDto.Adresa;
        user.Telefon = userDto.Telefon;

        await _userRepository.UpdateAsync(user);
        return MapToUserDto(user);
    }

    public async Task<bool> LoginAsync(UserLoginDto loginDto)
    {
        return await _userRepository.ValidateCredentialsAsync(loginDto.Email, loginDto.Password);
    }

    public async Task<bool> IsEmployeeAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null && user.Angajat;
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Email = user.Email,
            Nume = user.Nume,
            Prenume = user.Prenume,
            Adresa = user.Adresa,
            Telefon = user.Telefon,
            IsAngajat = user.Angajat
        };
    }
}

public interface IUserStateService
{
    event EventHandler UserStateChanged;

    bool IsLoggedIn { get; }
    bool IsEmployee { get; }
    string CurrentUserEmail { get; }
    UserDto CurrentUser { get; }

    Task<bool> LoginAsync(string email, string password);
    void Logout();
}

public class UserStateService : IUserStateService
{
    private readonly IUserService _userService;

    private UserDto _currentUser;
    public UserDto CurrentUser => _currentUser;

    public bool IsLoggedIn => _currentUser != null;
    public bool IsEmployee => _currentUser?.IsAngajat ?? false;
    public string CurrentUserEmail => _currentUser?.Email;

    public event EventHandler UserStateChanged;

    public UserStateService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var loginDto = new UserLoginDto
        {
            Email = email,
            Password = password
        };

        bool success = await _userService.LoginAsync(loginDto);

        if (success)
        {
            _currentUser = await _userService.GetUserByEmailAsync(email);
            OnUserStateChanged();
        }

        return success;
    }

    public void Logout()
    {
        _currentUser = null;
        OnUserStateChanged();

        // Debug message
        System.Diagnostics.Debug.WriteLine("UserStateService: User logged out");
    }

    protected virtual void OnUserStateChanged()
    {
        UserStateChanged?.Invoke(this, EventArgs.Empty);
    }
}