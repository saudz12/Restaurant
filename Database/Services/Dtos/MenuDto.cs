using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class MenuDto
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public string PozaUrl { get; set; }
    public CategoriiPreparate Categorie { get; set; }
    public List<MenuPreparatDto> ListaPreparate { get; set; } = new List<MenuPreparatDto>();
    public double PretTotal { get; set; }
    public bool IsAvailable { get; set; }
    public List<string> Alergeni { get; set; } = new List<string>();
}

public class MenuCreateDto
{
    public string Nume { get; set; }
    public string PozaUrl { get; set; }
    public CategoriiPreparate Categorie { get; set; }
    public List<MenuPreparatCreateDto> ListaPreparate { get; set; } = new List<MenuPreparatCreateDto>();
}

public class MenuPreparatCreateDto
{
    public int PreparatId { get; set; }
    public int Cantitate { get; set; }
}

public class MenuPreparatDto
{
    public int PreparatId { get; set; }
    public string PreparatNume { get; set; }
    public int Cantitate { get; set; }
    public int CantitatePortie { get; set; }
    public double PreparatPret { get; set; }
    public List<string> Alergeni { get; set; } = new List<string>();
}


public class MenuUpdateDto
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public string PozaUrl { get; set; }
    public CategoriiPreparate Categorie { get; set; }
    public List<MenuPreparatCreateDto> ListaPreparate { get; set; } = new List<MenuPreparatCreateDto>();
}