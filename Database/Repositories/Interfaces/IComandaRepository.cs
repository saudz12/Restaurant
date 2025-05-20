using Database.Enums;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories.Interfaces;

public interface IComandaRepository
{
    Task<List<Comanda>> GetAllAsync();
    Task<List<Comanda>> GetByUserEmailAsync(string email);
    Task<List<Comanda>> GetByStareAsync(StareComanda stare);
    Task<Comanda> GetByIdAsync(int id);
    Task<Comanda> GetByIdWithDetailsAsync(int id);
    Task<Comanda> AddAsync(Comanda comanda);
    Task UpdateAsync(Comanda comanda);
    Task UpdateStareAsync(int id, StareComanda stare);
    Task DeleteAsync(int id);
}