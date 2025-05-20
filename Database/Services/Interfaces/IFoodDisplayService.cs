using Database.Enums;
using Database.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Interfaces;

public interface IFoodDisplayService
{
    Task<List<FoodDisplayItem>> GetAllFoodItemsAsync();
    Task<List<FoodDisplayItem>> GetFoodItemsByCategorieAsync(CategoriiPreparate categorie);
    Task<FoodDisplayItem> GetFoodItemByIdAsync(int id, string type);
}