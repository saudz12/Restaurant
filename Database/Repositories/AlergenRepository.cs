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

public class AlergenRepository : IAlergenRepository
{
    private readonly RestaurantDatabaseContext _context;

    public AlergenRepository(RestaurantDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Alergen>> GetAllAsync()
    {
        return await _context.Alergeni.ToListAsync();
    }

    public async Task<Alergen> GetByIdAsync(int id)
    {
        return await _context.Alergeni.FindAsync(id);
    }

    public async Task<Alergen> GetByTipAsync(Alergeni tipAlergen)
    {
        return await _context.Alergeni
            .FirstOrDefaultAsync(a => a.TipAlergen == tipAlergen);
    }

    public async Task<Alergen> AddAsync(Alergen alergen)
    {
        _context.Alergeni.Add(alergen);
        await _context.SaveChangesAsync();
        return alergen;
    }

    public async Task UpdateAsync(Alergen alergen)
    {
        _context.Entry(alergen).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var alergen = await _context.Alergeni.FindAsync(id);
        if (alergen != null)
        {
            _context.Alergeni.Remove(alergen);
            await _context.SaveChangesAsync();
        }
    }
}