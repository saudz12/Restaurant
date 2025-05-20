using Database.Enums;
using Database.Services.Dtos;
using Database.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services;

public class FoodDisplayService : IFoodDisplayService
{
    private readonly IPreparatService _preparatService;
    private readonly IMenuService _menuService;

    public FoodDisplayService(IPreparatService preparatService, IMenuService menuService)
    {
        _preparatService = preparatService;
        _menuService = menuService;
    }

    public async Task<List<FoodDisplayItem>> GetAllFoodItemsAsync()
    {
        var result = new List<FoodDisplayItem>();

        var preparate = await _preparatService.GetAllPreparateAsync();
        foreach (var preparat in preparate)
        {
            result.Add(MapPreparatToFoodDisplayItem(preparat));
        }

        var meniuri = await _menuService.GetAllMeniuriAsync();
        foreach (var meniu in meniuri)
        {
            result.Add(MapMeniuToFoodDisplayItem(meniu));
        }

        return result;
    }

    public async Task<List<FoodDisplayItem>> GetFoodItemsByCategorieAsync(CategoriiPreparate categorie)
    {
        var result = new List<FoodDisplayItem>();

        var preparate = await _preparatService.GetPreparateByCategorieAsync(categorie);
        foreach (var preparat in preparate)
        {
            result.Add(MapPreparatToFoodDisplayItem(preparat));
        }

        var meniuri = await _menuService.GetMeniuriByCategorieAsync(categorie);
        foreach (var meniu in meniuri)
        {
            result.Add(MapMeniuToFoodDisplayItem(meniu));
        }

        return result;
    }

    public async Task<List<FoodDisplayItem>> SearchFoodItemsAsync(string searchTerm, List<string> excludedAllergens = null, CategoriiPreparate? categorie = null)
    {
        var allItems = categorie.HasValue
            ? await GetFoodItemsByCategorieAsync(categorie.Value)
            : await GetAllFoodItemsAsync();

        var filteredItems = allItems;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower().Trim();
            filteredItems = filteredItems.Where(item =>
                item.Nume.ToLower().Contains(searchTerm) ||
                (item.Tip?.ToLower()?.Contains(searchTerm) ?? false)
            ).ToList();
        }

        if (excludedAllergens != null && excludedAllergens.Any())
        {
            var normalizedExcludedAllergens = excludedAllergens
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.ToLower().Trim())
                .ToList();

            if (normalizedExcludedAllergens.Any())
            {
                filteredItems = filteredItems.Where(item =>
                    !item.Alergeni.Any(alergen =>
                        normalizedExcludedAllergens.Contains(alergen.ToLower()))
                ).ToList();
            }
        }

        return filteredItems;
    }

    public async Task<FoodDisplayItem> GetFoodItemByIdAsync(int id, string type)
    {
        if (type.Equals("Preparat", System.StringComparison.OrdinalIgnoreCase))
        {
            var preparat = await _preparatService.GetPreparatByIdAsync(id);
            if (preparat == null)
                return null;

            return MapPreparatToFoodDisplayItem(preparat);
        }
        else if (type.Equals("Menu", System.StringComparison.OrdinalIgnoreCase))
        {
            var meniu = await _menuService.GetMeniuByIdAsync(id);
            if (meniu == null)
                return null;

            return MapMeniuToFoodDisplayItem(meniu);
        }

        return null;
    }

    private FoodDisplayItem MapPreparatToFoodDisplayItem(PreparatDto preparat)
    {
        return new FoodDisplayItem
        {
            Id = preparat.Id,
            Nume = preparat.Nume,
            Pret = preparat.Pret,
            Categorie = preparat.Categorie,
            PozaUrl = preparat.PozaUrl,
            Tip = "Preparat",
            IsAvailable = preparat.IsAvailable,
            Alergeni = preparat.Alergeni.Select(a => a.Nume).ToList(),
            Continut = new List<FoodItemContentDto>
                {
                    new FoodItemContentDto
                    {
                        Nume = preparat.Nume,
                        Cantitate = preparat.CantitatePortie
                    }
                }
        };
    }

    private FoodDisplayItem MapMeniuToFoodDisplayItem(MenuDto meniu)
    {
        return new FoodDisplayItem
        {
            Id = meniu.Id,
            Nume = meniu.Nume,
            Pret = meniu.PretTotal, // This should now be calculated correctly
            Categorie = meniu.Categorie,
            PozaUrl = meniu.PozaUrl,
            Tip = "Menu",
            IsAvailable = meniu.IsAvailable,
            Alergeni = meniu.Alergeni,
            Continut = meniu.ListaPreparate.Select(mp => new FoodItemContentDto
            {
                Nume = mp.PreparatNume,
                Cantitate = mp.Cantitate
            }).ToList()
        };
    }

    public async Task<List<string>> GetAllAllergensAsync()
    {
        var allItems = await GetAllFoodItemsAsync();

        // Extract unique allergen names
        var allergens = allItems
            .SelectMany(item => item.Alergeni ?? new List<string>())
            .Distinct()
            .OrderBy(a => a)
            .ToList();

        return allergens;
    }
}
