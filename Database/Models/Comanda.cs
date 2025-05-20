using Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models;

public class Comanda
{
    public int                      Id                  { get; set; }
    public User                     User                { get; set; }
    public StareComanda             StareComanda        { get; set; }
    public List<PreparatCantitate>  PreparatCantitate   { get; set; } = [];
}