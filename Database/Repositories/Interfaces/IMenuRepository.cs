using Database.Enums;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories.Interfaces;

public interface IMenuRepository
{
    Task<List<Menu>> GetAllAsync();
    Task<List<Menu>> GetAllWithPreparateAsync();
    Task<List<Menu>> GetByCategorieAsync(CategoriiPreparate categorie);
    Task<Menu> GetByIdAsync(int id);
    Task<Menu> GetByIdWithPreparateAsync(int id);
    Task<Menu> AddAsync(Menu menu);
    Task UpdateAsync(Menu menu);
    Task DeleteAsync(int id);
}