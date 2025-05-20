using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByEmailWithComenziAsync(string email);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task<bool> ValidateCredentialsAsync(string email, string password);
}
