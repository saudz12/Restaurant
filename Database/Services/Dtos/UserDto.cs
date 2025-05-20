using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Services.Dtos;

public class UserDto
{
    public string Email { get; set; }
    public string Nume { get; set; }
    public string Prenume { get; set; }
    public string Adresa { get; set; }
    public string Telefon { get; set; }
    public bool IsAngajat { get; set; }
}

public class UserLoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserRegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Nume { get; set; }
    public string Prenume { get; set; }
    public string Adresa { get; set; }
    public string Telefon { get; set; }
    public bool IsAngajat { get; set; }
}

public class UserUpdateDto
{
    public string Email { get; set; }
    public string Nume { get; set; }
    public string Prenume { get; set; }
    public string Adresa { get; set; }
    public string Telefon { get; set; }
}