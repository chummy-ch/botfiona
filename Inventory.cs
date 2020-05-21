using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Fiona
{
  class Inventory
  {

    public Dictionary<string, string> playersInv = new Dictionary<string, string>();

    public Inventory(string inv, string uname)
    {
      if (playersInv.ContainsKey(uname)) playersInv[uname] = inv;
      else playersInv.Add(uname, inv);
    }

    public Inventory()
    {

    }

  }
}
