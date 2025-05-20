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

public class ComandaService : IComandaService
{
    private readonly IComandaRepository _comandaRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPreparatRepository _preparatRepository;

    public ComandaService(
        IComandaRepository comandaRepository,
        IUserRepository userRepository,
        IPreparatRepository preparatRepository)
    {
        _comandaRepository = comandaRepository;
        _userRepository = userRepository;
        _preparatRepository = preparatRepository;
    }

    public async Task<List<ComandaDto>> GetAllComenziAsync()
    {
        var comenzi = await _comandaRepository.GetAllAsync();

        var result = new List<ComandaDto>();
        foreach (var comanda in comenzi)
        {
            var comandaWithDetails = await _comandaRepository.GetByIdWithDetailsAsync(comanda.Id);
            result.Add(MapToComandaDto(comandaWithDetails));
        }

        return result;
    }

    public async Task<List<ComandaDto>> GetComenziByUserAsync(string email)
    {
        var comenzi = await _comandaRepository.GetByUserEmailAsync(email);
        return comenzi.Select(MapToComandaDto).ToList();
    }

    public async Task<List<ComandaDto>> GetComenziByStareAsync(StareComanda stare)
    {
        var comenzi = await _comandaRepository.GetByStareAsync(stare);

        var result = new List<ComandaDto>();
        foreach (var comanda in comenzi)
        {
            var comandaWithDetails = await _comandaRepository.GetByIdWithDetailsAsync(comanda.Id);
            result.Add(MapToComandaDto(comandaWithDetails));
        }

        return result;
    }

    public async Task<ComandaDto> GetComandaByIdAsync(int id)
    {
        var comanda = await _comandaRepository.GetByIdWithDetailsAsync(id);
        if (comanda == null)
            return null;

        return MapToComandaDto(comanda);
    }

    public async Task<ComandaDto> CreateComandaAsync(ComandaCreateDto comandaDto)
    {
        var user = await _userRepository.GetByEmailAsync(comandaDto.UserEmail);
        if (user == null)
            throw new KeyNotFoundException($"User with email {comandaDto.UserEmail} not found");

        var comanda = new Comanda
        {
            User = user,
            StareComanda = StareComanda.Inregistrata,
            PreparatCantitate = new List<PreparatCantitate>()
        };

        if (comandaDto.Items != null && comandaDto.Items.Any())
        {
            foreach (var item in comandaDto.Items)
            {
                var preparat = await _preparatRepository.GetByIdAsync(item.PreparatId);
                if (preparat == null)
                    throw new KeyNotFoundException($"Preparat with ID {item.PreparatId} not found");

                if (preparat.CantitateTotala < item.Cantitate)
                    throw new InvalidOperationException($"Insufficient quantity for preparat {preparat.Nume}");

                comanda.PreparatCantitate.Add(new PreparatCantitate
                {
                    PreparatId = item.PreparatId,
                    Preparat = preparat,
                    Cantitate = item.Cantitate
                });

                preparat.CantitateTotala -= item.Cantitate;
                await _preparatRepository.UpdateAsync(preparat);
            }
        }

        var createdComanda = await _comandaRepository.AddAsync(comanda);
        return MapToComandaDto(createdComanda);
    }

    public async Task UpdateComandaStareAsync(ComandaUpdateStareDto updateDto)
    {
        await _comandaRepository.UpdateStareAsync(updateDto.ComandaId, updateDto.StareComanda);
    }

    public async Task DeleteComandaAsync(int id)
    {
        var comanda = await _comandaRepository.GetByIdWithDetailsAsync(id);
        if (comanda == null)
            throw new KeyNotFoundException($"Comanda with ID {id} not found");

        foreach (var item in comanda.PreparatCantitate)
        {
            var preparat = await _preparatRepository.GetByIdAsync(item.PreparatId);
            if (preparat != null)
            {
                preparat.CantitateTotala += item.Cantitate;
                await _preparatRepository.UpdateAsync(preparat);
            }
        }

        await _comandaRepository.DeleteAsync(id);
    }

    private ComandaDto MapToComandaDto(Comanda comanda)
    {
        return new ComandaDto
        {
            Id = comanda.Id,
            UserEmail = comanda.User.Email,
            UserNume = comanda.User.Nume,
            UserPrenume = comanda.User.Prenume,
            UserAdresa = comanda.User.Adresa,
            UserTelefon = comanda.User.Telefon,
            StareComanda = comanda.StareComanda,
            Items = comanda.PreparatCantitate.Select(pc => new ComandaItemDto
            {
                PreparatId = pc.PreparatId,
                PreparatNume = pc.Preparat.Nume,
                Cantitate = pc.Cantitate,
                PretUnitar = pc.Preparat.Pret
            }).ToList(),
            DataCreare = DateTime.Now 
        };
    }
}
