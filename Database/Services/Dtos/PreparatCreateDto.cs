using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class PreparatCreateDto
{
    public string Nume { get; set; }
    public double Pret { get; set; }
    public int CantitatePortie { get; set; }
    public int CantitateTotala { get; set; }
    public CategoriiPreparate Categorie { get; set; }
    public string PozaUrl { get; set; }
    public List<int> AlergenIds { get; set; } = new List<int>();
}