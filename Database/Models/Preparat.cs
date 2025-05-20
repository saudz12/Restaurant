using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models;

public class Preparat
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public double Pret { get; set; }
    public int CantitatePortie { get; set; }
    public int CantitateTotala { get; set; }
    public CategoriiPreparate Categorie { get; set; }
    public string Poza { get; set; }
    public ICollection<Alergen> Alergeni { get; set; } = [];
}