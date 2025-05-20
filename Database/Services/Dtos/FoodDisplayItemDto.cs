using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class FoodDisplayItem
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public double Pret { get; set; }
    public CategoriiPreparate Categorie { get; set; }
    public string PozaUrl { get; set; }
    public string Tip { get; set; } // "Preparat" / "Menu"
    public bool IsAvailable { get; set; }
    public List<string> Alergeni { get; set; } = new List<string>();
    public List<FoodItemContentDto> Continut { get; set; } = new List<FoodItemContentDto>();
}

public class FoodItemContentDto
{
    public string Nume { get; set; }
    public int Cantitate { get; set; }
}