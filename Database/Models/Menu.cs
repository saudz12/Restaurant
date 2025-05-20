using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models;

public class Menu
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public string Poza { get; set; }
    public CategoriiPreparate Categorie { get; set; } 
    public ICollection<MenuPreparat> ListaPreparate { get; set; } = [];
}