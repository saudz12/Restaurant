using Database.Context;
using Database.Models;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RestaurantDatabaseContext _context;

    public UserRepository(RestaurantDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.FindAsync(email);
    }

    public async Task<User> GetByEmailWithComenziAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Comenzi)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string email)
    {
        var user = await _context.Users.FindAsync(email);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _context.Users.FindAsync(email);
        return user != null && user.Password == password;
    }
}