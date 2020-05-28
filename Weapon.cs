using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Fiona
{
  class Weapon
  {
    public Dictionary<string, int> weaponDamage = new Dictionary<string, int>() { { "Меч", 1 }, { "Щит", -1 }, { "Сабля", 2 }, { "Рапира", 3 } };

    public Weapon()
    {

    }

  }
}
