using Database.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Interfaces;

public interface IAlergenService
{
    Task<List<AlergenDto>> GetAllAlergeniAsync();
    Task<AlergenDto> GetAlergenByIdAsync(int id);
}
