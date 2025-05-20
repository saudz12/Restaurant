using Database.Enums;
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

public class PreparatService : IPreparatService
{
    private readonly IPreparatRepository _preparatRepository;
    private readonly IAlergenRepository _alergenRepository;

    public PreparatService(IPreparatRepository preparatRepository, IAlergenRepository alergenRepository)
    {
        _preparatRepository = preparatRepository;
        _alergenRepository = alergenRepository;
    }

    public async Task<List<PreparatDto>> GetAllPreparateAsync()
    {
        var preparate = await _preparatRepository.GetWithAlergeniAsync();
        return preparate.Select(MapToPreparatDto).ToList();
    }

    public async Task<List<PreparatDto>> GetPreparateByCategorieAsync(CategoriiPreparate categorie)
    {
        var preparate = await _preparatRepository.GetByCategorieAsync(categorie);

        // Load alergeni for each preparat
        var result = new List<PreparatDto>();
        foreach (var preparat in preparate)
        {
            var preparatWithAlergeni = await _preparatRepository.GetByIdAsync(preparat.Id);
            result.Add(MapToPreparatDto(preparatWithAlergeni));
        }

        return result;
    }

    public async Task<PreparatDto> GetPreparatByIdAsync(int id)
    {
        var preparat = await _preparatRepository.GetByIdAsync(id);
        if (preparat == null)
            return null;

        return MapToPreparatDto(preparat);
    }

    public async Task<PreparatDto> CreatePreparatAsync(PreparatCreateDto preparatDto)
    {
        var preparat = new Preparat
        {
            Nume = preparatDto.Nume,
            Pret = preparatDto.Pret,
            CantitatePortie = preparatDto.CantitatePortie,
            CantitateTotala = preparatDto.CantitateTotala,
            Categorie = preparatDto.Categorie,
            Poza = preparatDto.PozaUrl,
            Alergeni = new List<Alergen>()
        };

        // Add alergeni
        if (preparatDto.AlergenIds != null && preparatDto.AlergenIds.Any())
        {
            foreach (var alergenId in preparatDto.AlergenIds)
            {
                var alergen = await _alergenRepository.GetByIdAsync(alergenId);
                if (alergen != null)
                {
                    preparat.Alergeni.Add(alergen);
                }
            }
        }

        var createdPreparat = await _preparatRepository.AddAsync(preparat);
        return MapToPreparatDto(createdPreparat);
    }

    public async Task UpdatePreparatAsync(PreparatUpdateDto preparatDto)
    {
        var preparat = await _preparatRepository.GetByIdAsync(preparatDto.Id);
        if (preparat == null)
            throw new KeyNotFoundException($"Preparat with ID {preparatDto.Id} not found");

        preparat.Nume = preparatDto.Nume;
        preparat.Pret = preparatDto.Pret;
        preparat.CantitatePortie = preparatDto.CantitatePortie;
        preparat.CantitateTotala = preparatDto.CantitateTotala;
        preparat.Categorie = preparatDto.Categorie;
        preparat.Poza = preparatDto.PozaUrl;

        // Update alergeni
        preparat.Alergeni.Clear();
        if (preparatDto.AlergenIds != null && preparatDto.AlergenIds.Any())
        {
            foreach (var alergenId in preparatDto.AlergenIds)
            {
                var alergen = await _alergenRepository.GetByIdAsync(alergenId);
                if (alergen != null)
                {
                    preparat.Alergeni.Add(alergen);
                }
            }
        }

        await _preparatRepository.UpdateAsync(preparat);
    }

    public async Task DeletePreparatAsync(int id)
    {
        if (!await _preparatRepository.ExistsAsync(id))
            throw new KeyNotFoundException($"Preparat with ID {id} not found");

        await _preparatRepository.DeleteAsync(id);
    }

    public async Task<bool> IsPreparatAvailableAsync(int id, int requestedQuantity)
    {
        var availableQuantity = await _preparatRepository.GetAvailableQuantityAsync(id);
        return availableQuantity >= requestedQuantity;
    }

    public async Task UpdatePreparatQuantityAsync(int id, int newQuantity)
    {
        if (!await _preparatRepository.ExistsAsync(id))
            throw new KeyNotFoundException($"Preparat with ID {id} not found");

        await _preparatRepository.UpdateQuantityAsync(id, newQuantity);
    }

    private PreparatDto MapToPreparatDto(Preparat preparat)
    {
        return new PreparatDto
        {
            Id = preparat.Id,
            Nume = preparat.Nume,
            Pret = preparat.Pret,
            CantitatePortie = preparat.CantitatePortie,
            CantitateTotala = preparat.CantitateTotala,
            Categorie = preparat.Categorie,
            PozaUrl = preparat.Poza,
            Alergeni = preparat.Alergeni?.Select(a => new AlergenDto
            {
                Id = a.Id,
                Nume = a.Nume,
                TipAlergen = a.TipAlergen
            }).ToList() ?? new List<AlergenDto>()
        };
    }
}