using Database.Enums;
using Database.Models;
using Database.Repositories.Interfaces;
using Database.Services.Dtos;
using Database.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;
    private readonly IPreparatRepository _preparatRepository;
    private readonly IConfiguration _configuration;

    public MenuService(IMenuRepository menuRepository, IPreparatRepository preparatRepository, IConfiguration configuration)
    {
        _menuRepository = menuRepository;
        _preparatRepository = preparatRepository;
        _configuration = configuration;
    }

    public async Task<List<MenuDto>> GetAllMeniuriAsync()
    {
        var meniuri = await _menuRepository.GetAllWithPreparateAsync();
        return await MapToMenuDtos(meniuri);
    }

    public async Task<List<MenuDto>> GetMeniuriByCategorieAsync(CategoriiPreparate categorie)
    {
        var meniuri = await _menuRepository.GetByCategorieAsync(categorie);

        var result = new List<MenuDto>();
        foreach (var menu in meniuri)
        {
            var menuWithPreparate = await _menuRepository.GetByIdWithPreparateAsync(menu.Id);
            var menuDto = await MapToMenuDto(menuWithPreparate);
            result.Add(menuDto);
        }

        return result;
    }

    public async Task<MenuDto> GetMeniuByIdAsync(int id)
    {
        var menu = await _menuRepository.GetByIdWithPreparateAsync(id);
        if (menu == null)
            return null;

        return await MapToMenuDto(menu);
    }

    public async Task<MenuDto> CreateMeniuAsync(MenuCreateDto menuDto)
    {
        var menu = new Menu
        {
            Nume = menuDto.Nume,
            Poza = menuDto.PozaUrl,
            Categorie = menuDto.Categorie,
            ListaPreparate = new List<MenuPreparat>()
        };

        if (menuDto.ListaPreparate != null && menuDto.ListaPreparate.Any())
        {
            foreach (var item in menuDto.ListaPreparate)
            {
                var preparat = await _preparatRepository.GetByIdAsync(item.PreparatId);
                if (preparat != null)
                {
                    menu.ListaPreparate.Add(new MenuPreparat
                    {
                        PreparatId = item.PreparatId,
                        Preparat = preparat,
                        Cantitate = item.Cantitate
                    });
                }
            }
        }

        var createdMenu = await _menuRepository.AddAsync(menu);
        return await MapToMenuDto(createdMenu);
    }

    public async Task UpdateMeniuAsync(MenuUpdateDto menuDto)
    {
        var menu = await _menuRepository.GetByIdAsync(menuDto.Id);
        if (menu == null)
            throw new KeyNotFoundException($"Menu with ID {menuDto.Id} not found");

        menu.Nume = menuDto.Nume;
        menu.Poza = menuDto.PozaUrl;
        menu.Categorie = menuDto.Categorie;

        // Update preparate
        menu.ListaPreparate.Clear();
        if (menuDto.ListaPreparate != null && menuDto.ListaPreparate.Any())
        {
            foreach (var item in menuDto.ListaPreparate)
            {
                var preparat = await _preparatRepository.GetByIdAsync(item.PreparatId);
                if (preparat != null)
                {
                    menu.ListaPreparate.Add(new MenuPreparat
                    {
                        MenuId = menu.Id,
                        PreparatId = item.PreparatId,
                        Preparat = preparat,
                        Cantitate = item.Cantitate
                    });
                }
            }
        }

        await _menuRepository.UpdateAsync(menu);
    }

    public async Task DeleteMeniuAsync(int id)
    {
        var menu = await _menuRepository.GetByIdAsync(id);
        if (menu == null)
            throw new KeyNotFoundException($"Menu with ID {id} not found");

        await _menuRepository.DeleteAsync(id);
    }

    public async Task<bool> IsMeniuAvailableAsync(int id)
    {
        var menu = await _menuRepository.GetByIdWithPreparateAsync(id);
        if (menu == null)
            return false;

        // Check if all preparate in menu are available in sufficient quantities
        foreach (var menuPreparat in menu.ListaPreparate)
        {
            var preparat = await _preparatRepository.GetByIdAsync(menuPreparat.PreparatId);
            if (preparat == null || preparat.CantitateTotala < menuPreparat.Cantitate)
            {
                return false;
            }
        }

        return true;
    }

    public double CalculateMeniuPrice(List<MenuPreparatDto> preparate)
    {
        double totalPrice = preparate.Sum(p =>
            p.PreparatPret * ((double)p.Cantitate / p.CantitatePortie));

        double discount = GetMenuDiscount();
        return totalPrice * (1 - discount);
    }

    public double GetMenuDiscount()
    {
        double discount = 0.1; 

        string discountStr = _configuration["MenuDiscount"];
        if (!string.IsNullOrEmpty(discountStr) && double.TryParse(discountStr, out double configDiscount))
        {
            discount = configDiscount;
        }

        return discount;
    }

    private async Task<List<MenuDto>> MapToMenuDtos(List<Menu> meniuri)
    {
        var result = new List<MenuDto>();

        foreach (var menu in meniuri)
        {
            result.Add(await MapToMenuDto(menu));
        }

        return result;
    }

    private async Task<MenuDto> MapToMenuDto(Menu menu)
    {
        var preparateList = new List<MenuPreparatDto>();
        var alergeni = new HashSet<string>();
        bool isAvailable = true;

        foreach (var menuPreparat in menu.ListaPreparate)
        {
            var preparat = menuPreparat.Preparat;

            if (preparat.CantitateTotala < menuPreparat.Cantitate)
            {
                isAvailable = false;
            }

            foreach (var alergen in preparat.Alergeni)
            {
                alergeni.Add(alergen.Nume);
            }

            preparateList.Add(new MenuPreparatDto
            {
                PreparatId = preparat.Id,
                PreparatNume = preparat.Nume,
                Cantitate = menuPreparat.Cantitate,
                CantitatePortie = preparat.CantitatePortie,
                PreparatPret = preparat.Pret,
                Alergeni = preparat.Alergeni?.Select(a => a.Nume).ToList() ?? new List<string>()
            });
        }

        var menuDto = new MenuDto
        {
            Id = menu.Id,
            Nume = menu.Nume,
            PozaUrl = menu.Poza,
            Categorie = menu.Categorie,
            ListaPreparate = preparateList,
            Alergeni = alergeni.ToList(),
            IsAvailable = isAvailable
        };

        menuDto.PretTotal = CalculateMeniuPrice(preparateList);

        return menuDto;
    }
}
