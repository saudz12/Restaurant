using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class ComandaCreateDto
{
    public string UserEmail { get; set; }
    public List<ComandaItemCreateDto> Items { get; set; } = new List<ComandaItemCreateDto>();
}