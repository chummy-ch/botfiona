using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace botfiona
{
  class AccessManager
  {
    List<int> chatId = new List<int>();

    private long id;
    private bool action = false;
    public AccessManager(long Id, bool Action)
    {
      this.id = Id;
      this.action = Action;
    }
    public AccessManager(long Id)
    {
      this.id = Id;
    }

    public bool CheckAccedd()
    {
      return false;
    }
  }
}
