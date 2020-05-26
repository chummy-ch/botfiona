using botfiona;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot_Fiona
{
  class Inventory
  {
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;
    public Dictionary<string, string> totalwin;
    public Dictionary<string, string> weapon;

    public Inventory()
    {
      totalwin = new Dictionary<string, string>();
      LoadTotalWin();
      weapon = new Dictionary<string, string>();
      LoadWeapon();
    }

    public void ChangeWeapon(string w, string uname)
    {
      if (weapon.ContainsKey(uname)) weapon[uname] = w;
      else weapon.Add(uname, w);
      SaveWeapon();
    }



    public void AddPresent(string uName, int winId, string unamenow)
    {
      Roulette r = new Roulette();
      if (totalwin.ContainsKey(uName))
      {
        if (totalwin[uName].Contains("💰") && r.presents.ElementAt(winId).Key.Contains("💰"))
        {
          int index = totalwin[uName].IndexOf("💰");
          string old = totalwin[uName].Substring(index - 4, 5);
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
          totalwin[uName] = totalwin[uName].Replace(old, new1);
        }
        else if (r.presents.ElementAt(winId).Key.Contains("💰") && !totalwin[uName].Contains("💰"))
        {
          string t = totalwin[uName];
          totalwin[uName] = t + ":" + r.presents.ElementAt(winId).Key;
        }
        else if (totalwin[uName].Contains(r.presents.ElementAt(winId).Key.Substring(0, 5)) && r.presents.ElementAt(winId).Key.Contains("x"))
        {
          int index = totalwin[uName].IndexOf("x") + 1;
          string test = r.presents.ElementAt(winId).Key.Substring(0, 3);
          if (totalwin[uName].IndexOf(test) > index)
          {
            string ntest = r.presents.ElementAt(winId).Key;
            index = totalwin[uName].IndexOf(test) + ntest.IndexOf("x") + 1;
          }
          string old = totalwin[uName].Substring(index - 5, 7);
          int ind = old.IndexOf("x") + 1;
          string oldn = old.Substring(ind, 2);
          oldn = oldn.Trim();
          int newnubmer = Convert.ToInt32(oldn) + 1;
          string new1 = old.Replace(oldn.ToString(), newnubmer.ToString());
          totalwin[uName] = totalwin[uName].Replace(old, new1);
        }
        else if (r.presents.ElementAt(winId).Key.Contains("x") && !totalwin[uName].Contains(r.presents.ElementAt(winId).Key.Substring(0, 5)))
        {
          string t = totalwin[uName];
          totalwin[uName] = t + ":" + r.presents.ElementAt(winId).Key;
        }

      }
      else totalwin.Add(unamenow, $":{r.presents.ElementAt(winId).Key}");
      SaveTotalWin();
    }

    public async void Equip(string uName)
    {
      Console.WriteLine("E");
      if (!totalwin.ContainsKey(uName)) return;
      string[] arr = totalwin[uName].Split(':');
      /*InlineKeyboardMarkup eq = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithCallbackData("1")
        }
      });*/
      var q = new InlineKeyboardButton[arr.Length];
      Console.WriteLine(1);
      var k = new InlineKeyboardMarkup[1];
      Console.WriteLine(2);
      Console.WriteLine($".... {arr.Length}");
      int c = 0;
      for (int i = 0; i < arr.Length; i++)
      {
        Console.WriteLine(3);
        if(arr.Contains("x"))
        {
          c++; 
          q[i] = new InlineKeyboardButton
          {

            Text = arr[i],
            CallbackData = i.ToString()
          };
        }
        
      }
      q.Length = c; 
      Console.WriteLine(6);
      var key = new InlineKeyboardMarkup(q);
      Console.WriteLine(7);
      await Bot.SendTextMessageAsync(m.Message.From.Id, "pines", replyMarkup: key);
      Console.WriteLine(8);

    }

    public void GetInventory()
    {
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

    private void LoadWeapon()
    {

      if (!System.IO.File.Exists("Weapon.txt")) return;
      string json = System.IO.File.ReadAllText("Weapon.txt");
      weapon = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json);
    }

    private void SaveWeapon()
    {
      using (StreamWriter writer = System.IO.File.CreateText("Weapons.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, weapon);
      }
    }

    private void SaveTotalWin()
    {
      using (StreamWriter writer = System.IO.File.CreateText("TotalWin.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, totalwin);
      }
    }

    private void LoadTotalWin()
    {
      if (!System.IO.File.Exists("TotalWin.txt")) return;
      string json = System.IO.File.ReadAllText("TotalWin.txt");
      totalwin = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json);
    }

  }
}
