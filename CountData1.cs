using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace botfiona
{
    [Serializable]
    class CountData
    {
        public int Count { get; set; }

        public CountData(int count)
        {
            Count = count;
        }

    }
}
