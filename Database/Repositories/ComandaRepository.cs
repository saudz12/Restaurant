using Database.Context;
using Database.Enums;
using Database.Models;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public class ComandaRepository : IComandaRepository
{
    private readonly RestaurantDatabaseContext _context;

    public ComandaRepository(RestaurantDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Comanda>> GetAllAsync()
    {
        return await _context.Comenzi
            .Include(c => c.User)
            .ToListAsync();
    }

    public async Task<List<Comanda>> GetByUserEmailAsync(string email)
    {
        return await _context.Comenzi
            .Include(c => c.User)
            .Include(c => c.PreparatCantitate)
                .ThenInclude(pc => pc.Preparat)
            .Where(c => c.User.Email == email)
            .ToListAsync();
    }

    public async Task<List<Comanda>> GetByStareAsync(StareComanda stare)
    {
        return await _context.Comenzi
            .Include(c => c.User)
            .Where(c => c.StareComanda == stare)
            .ToListAsync();
    }

    public async Task<Comanda> GetByIdAsync(int id)
    {
        return await _context.Comenzi.FindAsync(id);
    }

    public async Task<Comanda> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Comenzi
            .Include(c => c.User)
            .Include(c => c.PreparatCantitate)
                .ThenInclude(pc => pc.Preparat)
                    .ThenInclude(p => p.Alergeni)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Comanda> AddAsync(Comanda comanda)
    {
        _context.Comenzi.Add(comanda);
        await _context.SaveChangesAsync();
        return comanda;
    }

    public async Task UpdateAsync(Comanda comanda)
    {
        _context.Entry(comanda).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStareAsync(int id, StareComanda stare)
    {
        var comanda = await _context.Comenzi.FindAsync(id);
        if (comanda != null)
        {
            comanda.StareComanda = stare;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var comanda = await _context.Comenzi.FindAsync(id);
        if (comanda != null)
        {
            _context.Comenzi.Remove(comanda);
            await _context.SaveChangesAsync();
        }
    }
}