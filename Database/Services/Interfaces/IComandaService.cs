using Database.Enums;
using Database.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Interfaces;

public interface IComandaService
{
    Task<List<ComandaDto>> GetAllComenziAsync();
    Task<List<ComandaDto>> GetComenziByUserAsync(string email);
    Task<List<ComandaDto>> GetComenziByStareAsync(StareComanda stare);
    Task<ComandaDto> GetComandaByIdAsync(int id);
    Task<ComandaDto> CreateComandaAsync(ComandaCreateDto comandaDto);
    Task UpdateComandaStareAsync(ComandaUpdateStareDto updateDto);
    Task DeleteComandaAsync(int id);
}

