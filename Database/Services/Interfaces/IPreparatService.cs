using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Services.Dtos;

namespace Database.Services.Interfaces;

public interface IPreparatService
{
    Task<List<PreparatDto>> GetAllPreparateAsync();
    Task<List<PreparatDto>> GetPreparateByCategorieAsync(CategoriiPreparate categorie);
    Task<PreparatDto> GetPreparatByIdAsync(int id);
    Task<PreparatDto> CreatePreparatAsync(PreparatCreateDto preparatDto);
    Task UpdatePreparatAsync(PreparatUpdateDto preparatDto);
    Task DeletePreparatAsync(int id);
    Task<bool> IsPreparatAvailableAsync(int id, int requestedQuantity);
    Task UpdatePreparatQuantityAsync(int id, int newQuantity);
}