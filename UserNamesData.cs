using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace botfiona
{
    [Serializable]
    class UserNamesData
    {
        public string UName { get; set; }
        
        public UserNamesData(string uname)
        {
            UName = uname;
        }
    }
}
