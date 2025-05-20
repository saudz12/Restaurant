using Database.Models;
using Database.Repositories.Interfaces;
using Database.Services.Dtos;
using Database.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services;

public class AlergenService : IAlergenService
{
    private readonly IAlergenRepository _alergenRepository;

    public AlergenService(IAlergenRepository alergenRepository)
    {
        _alergenRepository = alergenRepository;
    }

    public async Task<List<AlergenDto>> GetAllAlergeniAsync()
    {
        var alergeni = await _alergenRepository.GetAllAsync();
        return alergeni.Select(MapToAlergenDto).ToList();
    }

    public async Task<AlergenDto> GetAlergenByIdAsync(int id)
    {
        var alergen = await _alergenRepository.GetByIdAsync(id);
        if (alergen == null)
            return null;

        return MapToAlergenDto(alergen);
    }

    private AlergenDto MapToAlergenDto(Alergen alergen)
    {
        return new AlergenDto
        {
            Id = alergen.Id,
            Nume = alergen.Nume,
            TipAlergen = alergen.TipAlergen
        };
    }
}
