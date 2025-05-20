using Database.Enums;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories.Interfaces;

public interface IPreparatRepository
{
    Task<List<Preparat>> GetAllAsync();
    Task<List<Preparat>> GetByCategorieAsync(CategoriiPreparate categorie);
    Task<Preparat> GetByIdAsync(int id);
    Task<List<Preparat>> GetWithAlergeniAsync();
    Task<Preparat> AddAsync(Preparat preparat);
    Task UpdateAsync(Preparat preparat);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> GetAvailableQuantityAsync(int preparatId);
    Task UpdateQuantityAsync(int preparatId, int newQuantity);
}
