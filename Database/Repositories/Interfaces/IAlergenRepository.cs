using Database.Enums;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories.Interfaces;

public interface IAlergenRepository
{
    Task<List<Alergen>> GetAllAsync();
    Task<Alergen> GetByIdAsync(int id);
    Task<Alergen> GetByTipAsync(Alergeni tipAlergen);
    Task<Alergen> AddAsync(Alergen alergen);
    Task UpdateAsync(Alergen alergen);
    Task DeleteAsync(int id);
}
