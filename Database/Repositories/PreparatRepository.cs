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

public class PreparatRepository : IPreparatRepository
{
    private readonly RestaurantDatabaseContext _context;

    public PreparatRepository(RestaurantDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Preparat>> GetAllAsync()
    {
        return await _context.Preparate.ToListAsync();
    }

    public async Task<List<Preparat>> GetByCategorieAsync(CategoriiPreparate categorie)
    {
        return await _context.Preparate
            .Where(p => p.Categorie == categorie)
            .ToListAsync();
    }

    public async Task<Preparat> GetByIdAsync(int id)
    {
        return await _context.Preparate
            .Include(p => p.Alergeni)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Preparat>> GetWithAlergeniAsync()
    {
        return await _context.Preparate
            .Include(p => p.Alergeni)
            .ToListAsync();
    }

    public async Task<Preparat> AddAsync(Preparat preparat)
    {
        _context.Preparate.Add(preparat);
        await _context.SaveChangesAsync();
        return preparat;
    }

    public async Task UpdateAsync(Preparat preparat)
    {
        _context.Entry(preparat).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var preparat = await _context.Preparate.FindAsync(id);
        if (preparat != null)
        {
            _context.Preparate.Remove(preparat);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Preparate.AnyAsync(p => p.Id == id);
    }

    public async Task<int> GetAvailableQuantityAsync(int preparatId)
    {
        var preparat = await _context.Preparate.FindAsync(preparatId);
        return preparat?.CantitateTotala ?? 0;
    }

    public async Task UpdateQuantityAsync(int preparatId, int newQuantity)
    {
        var preparat = await _context.Preparate.FindAsync(preparatId);
        if (preparat != null)
        {
            preparat.CantitateTotala = newQuantity;
            await _context.SaveChangesAsync();
        }
    }
}