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
    private int freshboard;

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

    private async void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      if (e.CallbackQuery.Message.MessageId != freshboard) return;
      ChangeWeapon(e.CallbackQuery.Data, e.CallbackQuery.From.Username);
      await Bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, freshboard);
      await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы выбрали {e.CallbackQuery.Data}");
    }

    public void AddPresent(string uName, int winId, string unamenow)
    {
      Roulette r = new Roulette();
      if (totalwin.ContainsKey(uName))
      {
        if (totalwin[uName].Contains(r.presents.ElementAt(winId).Key)) return;
        else
        {
          totalwin[uName] = totalwin[uName] + $",{r.presents.ElementAt(winId).Key}";
        }
      }
      else totalwin.Add(unamenow, $"{r.presents.ElementAt(winId).Key}");
      SaveTotalWin();
    }

   
    public string GetWeapon(string uName)
    {
      if (weapon.ContainsKey(uName)) return weapon[uName];
      else return "";
    }

    public async void GetInventory(string uName)
    {
      Bot.OnCallbackQuery += bot_OnCallbackQuery;
      if (!totalwin.ContainsKey(uName)) return;
      string[] arr = totalwin[uName].Split(',');
      var q = new InlineKeyboardButton[arr.Length];
      var k = new InlineKeyboardButton[1][];
      for (int i = 0; i < arr.Length; i++)
      {
        q[i] = new InlineKeyboardButton
        {
          Text = arr[i],
          CallbackData = arr[i],
        };
      }
      k[0] = q;
      var key = new InlineKeyboardMarkup(k);
      freshboard = Bot.SendTextMessageAsync(m.Message.From.Id, "Ваше оружие", replyMarkup: key).Result.MessageId;
      await Bot.SendTextMessageAsync(m.Message.From.Id, "Нажмите, чтобы выбрать");
    }

    private void LoadWeapon()
    {

      if (!System.IO.File.Exists("Weapons.txt")) return;
      string json = System.IO.File.ReadAllText("Weapons.txt");
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
