using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class ComandaUpdateStareDto
{
    public int ComandaId { get; set; }
    public StareComanda StareComanda { get; set; }
}
