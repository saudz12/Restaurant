using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class ComandaDto
{
    public int Id { get; set; }
    public string UserEmail { get; set; }
    public string UserNume { get; set; }
    public string UserPrenume { get; set; }
    public string UserAdresa { get; set; }
    public string UserTelefon { get; set; }
    public StareComanda StareComanda { get; set; }
    public List<ComandaItemDto> Items { get; set; } = new List<ComandaItemDto>();
    public double PretTotal => Items.Sum(i => i.PretTotal);
    public DateTime DataCreare { get; set; }
}