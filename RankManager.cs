﻿using System;
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
                { 5000, "Харьковчанин" }, { 10000, "Хнурэшник" },
                { 25000, "Языковая шаболда" }, { 88888, "Болотный барт" },
                { 100000, "Салтовчанен" }
            };

      ranksPics = new Dictionary<string, string>()
            {
              {"Чел", "http://pushkin.ellink.ru/2018/pushkin/images/fullscreen/pushkin.jpg"}, { "Глеб", "https://i1.rgstatic.net/ii/profile.image/797915803029504-1567249360132_Q512/Glib_Tereshchenko.jpg" } ,
              {"Конч", "https://i1.rgstatic.net/ii/profile.image/797915803029504-1567249360132_Q512/Glib_Tereshchenko.jpg" }, {"Мамонтов", "https://nure.ua/wp-content/uploads/Employees_photo/mamontov.jpg"},
              {"Харьковчанин", "http://bardak.kharkov.ua/wp-content/uploads/2017/09/08-09-01-1.jpg"}, {"Хнурэшник", "https://telegra.ph/file/5e7b018be301866334cbd.jpg"},
              {"Языковая шаболда", "https://nure.ua/wp-content/uploads/Employees_photo/LitvinAG.jpg" }
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

    public string GetPic(int mescount)
    {
      string rank = "";
      for (int i = 0; i < numRanks.Count; i ++)
      {
        if (mescount >= numRanks.ElementAt(i).Key && mescount < numRanks.ElementAt(i + 1).Key)
        {
          rank = numRanks.ElementAt(i).Value;
          break;
        }
      }
      return ranksPics[rank];
    }
  }

}