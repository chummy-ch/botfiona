using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace botfiona
{
    class RankManager
    {
        Dictionary<int, string> numRanks;
        Dictionary<string, string> ranksPics;

        public RankManager()
        {
            numRanks = new Dictionary<int, string>()
            {
                {0, "Чел" }, { 150, "Глеб" }, { 800, "Конч" }, { 2000, "Мамонтов" },
                { 5000, "Харьковчанен" }, { 10000, "Хнурэшник" },
                { 25000, "Языковая шаболда" }, { 88888, "Болотный барт" },
                { 100000, "Салтовчанен" }
            };

            ranksPics = new Dictionary<string, string>()
            {
                { "Глеб", "https://i1.rgstatic.net/ii/profile.image/797915803029504-1567249360132_Q512/Glib_Tereshchenko.jpg" }
            };
        }

        public string GetFormattedString(int userMsgCount, string userName)
        {
            string rank = "";
            for (int i = 0; i < numRanks.Count; i++)
            {
                if (userMsgCount > numRanks.Keys.ElementAt(i))
                    rank = numRanks.Values.ElementAt(i);
            }
            return string.Format("Юзер: @{0} \nРанг: {1} \nКол-во сообщений: {2}", userName, rank, userMsgCount);
        }

        public bool CountExists(int msgUser)
        {
            if (numRanks.ContainsKey(msgUser)) return true;
            else return false;
        }

        public string GetRank(int index)
        {
            return numRanks[index];
        }
    }


  class Hueta : RankManager
  {
    Hueta()
    {

    }
  }
}