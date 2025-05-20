using Database.Enums;
using Database.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Interfaces;

public interface IMenuService
{
    Task<List<MenuDto>> GetAllMeniuriAsync();
    Task<List<MenuDto>> GetMeniuriByCategorieAsync(CategoriiPreparate categorie);
    Task<MenuDto> GetMeniuByIdAsync(int id);
    Task<MenuDto> CreateMeniuAsync(MenuCreateDto menuDto);
    Task UpdateMeniuAsync(MenuUpdateDto menuDto);
    Task DeleteMeniuAsync(int id);
    Task<bool> IsMeniuAvailableAsync(int id);
    double CalculateMeniuPrice(List<MenuPreparatDto> preparate);
    double GetMenuDiscount();
}
