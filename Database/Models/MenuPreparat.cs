using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models;
public class MenuPreparat
{
    public int Id { get; set; }
    public int PreparatId { get; set; }
    public Preparat Preparat { get; set; }
    public int Cantitate { get; set; } 
    public int MenuId { get; set; }
}
