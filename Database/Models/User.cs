using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models;

public class User
{
    [Key]
    [Required]
    public string           Email       { get; set; }
    public string           Password    { get; set; }
    public string           Nume        { get; set; }
    public string           Prenume     { get; set; }
    public string           Adresa      { get; set; }
    public string           Telefon     { get; set; }
    public bool             Angajat     { get; set; }
    public List<Comanda>    Comenzi     { get; set; } = [];
}

