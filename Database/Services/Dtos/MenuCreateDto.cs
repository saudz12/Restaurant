using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class MenuCreateDto
{
    public string Nume { get; set; }
    public string PozaUrl { get; set; }
    public CategoriiPreparate Categorie { get; set; }
    public List<MenuPreparatCreateDto> ListaPreparate { get; set; } = new List<MenuPreparatCreateDto>();
}