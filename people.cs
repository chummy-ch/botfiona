using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace botfiona
{
    [Serializable]
    class PeopleData
    {
        public string People { get; set;  }

        public PeopleData(string people)
        {
            People = people;
        }

    }
}
