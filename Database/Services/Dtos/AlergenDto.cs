using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class AlergenDto
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public Alergeni TipAlergen { get; set; }
}
