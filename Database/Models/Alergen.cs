using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models;
public class Alergen
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public Enums.Alergeni TipAlergen { get; set; }
    public ICollection<Preparat> Preparate { get; set; } = [];
}
