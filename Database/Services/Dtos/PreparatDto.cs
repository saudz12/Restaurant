using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class PreparatDto
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public double Pret { get; set; }
    public int CantitatePortie { get; set; }
    public int CantitateTotala { get; set; }
    public bool IsAvailable => CantitateTotala >= CantitatePortie;
    public CategoriiPreparate Categorie { get; set; }
    public string PozaUrl { get; set; }
    public List<AlergenDto> Alergeni { get; set; } = new List<AlergenDto>();
}