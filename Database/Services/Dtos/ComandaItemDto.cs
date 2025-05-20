using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class ComandaItemDto
{
    public int PreparatId { get; set; }
    public string PreparatNume { get; set; }
    public int Cantitate { get; set; }
    public double PretUnitar { get; set; }
    public double PretTotal => PretUnitar * Cantitate;
}
