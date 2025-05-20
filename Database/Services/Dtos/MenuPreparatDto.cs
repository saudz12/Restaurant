using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class MenuPreparatDto
{
    public int PreparatId { get; set; }
    public string PreparatNume { get; set; }
    public int Cantitate { get; set; }
    public double PreparatPret { get; set; }
    public List<string> Alergeni { get; set; } = new List<string>();
}
