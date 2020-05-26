using botfiona;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Bot_Fiona
{
  class Inventory
  {
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;
    public Dictionary<string, string> totalwin;

    public Inventory(TelegramBotClient bot)
    {
      Bot = bot;
      totalwin = new Dictionary<string, string>();
    }

    public void AddPresent(string name, int winId, string unamenow)
    {
      Roulette r = new Roulette();
      if (totalwin.ContainsKey(name))
      {
        if (totalwin[name].Contains("💰") && r.presents.ElementAt(winId).Key.Contains("💰"))
        {
          int index = totalwin[name].IndexOf("💰");
          string old = totalwin[name].Substring(index - 4, 5);
          if (old[0].ToString() == ":")
          {
            old = old.Substring(1, 4);
          }
          else if (old[1].ToString() == ":")
          {
            old = old.Substring(2, 3);
          }
          string oldn = old.Substring(0, old.Length - 2);
          oldn = oldn.Trim();
          string newn = Convert.ToString(Convert.ToInt32(oldn) + 3);
          string new1 = old.Replace(oldn.ToString(), newn.ToString());
          totalwin[name] = totalwin[name].Replace(old, new1);
        }
        else if (r.presents.ElementAt(winId).Key.Contains("💰") && !totalwin[name].Contains("💰"))
        {
          string t = totalwin[name];
          totalwin[name] = t + ":" + r.presents.ElementAt(winId).Key;
        }
        else if (totalwin[name].Contains(r.presents.ElementAt(winId).Key.Substring(0, 5)) && presents.ElementAt(winId).Key.Contains("x"))
        {
          int index = totalwin[name].IndexOf("x") + 1;
          string test = r.presents.ElementAt(winId).Key.Substring(0, 3);
          if (totalwin[name].IndexOf(test) > index)
          {
            string ntest = r.presents.ElementAt(winId).Key;
            index = totalwin[name].IndexOf(test) + ntest.IndexOf("x") + 1;
          }
          string old = totalwin[name].Substring(index - 5, 7);
          int ind = old.IndexOf("x") + 1;
          string oldn = old.Substring(ind, 2);
          oldn = oldn.Trim();
          int newnubmer = Convert.ToInt32(oldn) + 1;
          string new1 = old.Replace(oldn.ToString(), newnubmer.ToString());
          totalwin[name] = totalwin[name].Replace(old, new1);
        }
        else if (r.presents.ElementAt(winId).Key.Contains("x") && !totalwin[name].Contains(r.presents.ElementAt(winId).Key.Substring(0, 5)))
        {
          string t = totalwin[name];
          totalwin[name] = t + ":" + r.presents.ElementAt(winId).Key;
        }

      }
      else totalwin.Add(unamenow, $":{r.presents.ElementAt(winId).Key}");
      SaveTotalWin();
    }

    public void GetInventory()
    {
      LoadTotalWin();
      string uName = m.Message.From.Username;
      if (!totalwin.ContainsKey(uName)) return;
      string[] arr = totalwin[uName].Split(':');
      string t = "Призы за все время:";
      for (int i = 0; i < arr.Count(); i++)
      {
        t += $"\n{arr[i]}";
      }
      Bot.SendTextMessageAsync(m.Message.From.Id, t);
    }


    private void SaveTotalWin()
    {
      using (StreamWriter writer = File.CreateText("TotalWin.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, totalwin);
      }
    }

    private void LoadTotalWin()
    {
      if (!File.Exists("TotalWin.txt")) return;
      string json = File.ReadAllText("TotalWin.txt");
      totalwin = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json);
    }

  }
}
