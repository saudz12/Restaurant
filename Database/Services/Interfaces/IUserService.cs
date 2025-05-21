using Database.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByEmailAsync(string email);
    Task<UserDto> RegisterUserAsync(UserRegisterDto userDto);
    Task<UserDto> UpdateUserAsync(UserUpdateDto userDto);
    Task<bool> LoginAsync(UserLoginDto loginDto);
    Task<bool> IsEmployeeAsync(string email);
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
