using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class UserUpdateDto
{
    public string Email { get; set; }
    public string Nume { get; set; }
    public string Prenume { get; set; }
    public string Adresa { get; set; }
    public string Telefon { get; set; }
}
