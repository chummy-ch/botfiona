using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Fiona
{
  class Weapon
  {

    private string weapon { get; set; }
    private int damage { get; set; }
    private int amount { get; set; }
    private bool equip { get; set; }


    public Weapon(string w, int d, int a, bool e, string uname)
    {
      weapon = w;
      damage = d;
      amount = a;
      equip = e;
      ToInv(uname);
    }

    private void ToInv(string uname)
    {
      Inventory inv = new Inventory();
      if (inv.playersInv.ContainsKey(uname))
      {
        string inventory = inv.playersInv[uname];
        
      }
    }
  }
}
