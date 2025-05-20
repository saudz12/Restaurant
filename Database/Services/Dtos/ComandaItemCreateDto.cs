using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class ComandaItemCreateDto
{
    public int PreparatId { get; set; }
    public int Cantitate { get; set; }
}
