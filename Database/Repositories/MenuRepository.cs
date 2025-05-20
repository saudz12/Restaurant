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

public class MenuRepository : IMenuRepository
{
    private readonly RestaurantDatabaseContext _context;

    public MenuRepository(RestaurantDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Menu>> GetAllAsync()
    {
        return await _context.Meniuri.ToListAsync();
    }

    public async Task<List<Menu>> GetAllWithPreparateAsync()
    {
        return await _context.Meniuri
            .Include(m => m.ListaPreparate)
                .ThenInclude(mp => mp.Preparat)
                    .ThenInclude(p => p.Alergeni)
            .ToListAsync();
    }

    public async Task<List<Menu>> GetByCategorieAsync(CategoriiPreparate categorie)
    {
        return await _context.Meniuri
            .Where(m => m.Categorie == categorie)
            .ToListAsync();
    }

    public async Task<Menu> GetByIdAsync(int id)
    {
        return await _context.Meniuri.FindAsync(id);
    }

    public async Task<Menu> GetByIdWithPreparateAsync(int id)
    {
        return await _context.Meniuri
            .Include(m => m.ListaPreparate)
                .ThenInclude(mp => mp.Preparat)
                    .ThenInclude(p => p.Alergeni)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Menu> AddAsync(Menu menu)
    {
        _context.Meniuri.Add(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    public async Task UpdateAsync(Menu menu)
    {
        //!!!n:m solved by intermediate table
        //remove existing MenuPreparat entries to avoid duplicates
        var existingMenuItems = await _context.MenuPreparat
            .Where(mp => mp.MenuId == menu.Id)
            .ToListAsync();

        _context.MenuPreparat.RemoveRange(existingMenuItems);

        //update the menu and add new MenuPreparat entries
        _context.Entry(menu).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await _context.Meniuri.FindAsync(id);
        if (menu != null)
        {
            _context.Meniuri.Remove(menu);
            await _context.SaveChangesAsync();
        }
    }
}