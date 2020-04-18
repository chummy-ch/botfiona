using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace botfiona
{
  class Roulette
  {
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;
    private long id1;
    private int id2;
    private bool status = false;
    
    private Dictionary<string, string> presents = new Dictionary<string, string>() { { "Рапира x1 ", "01" }, { "Короткий меч x1 ", "04070915" }, { "Сабля x1 ", "051217" }, { "3 💰", "02030608101113141618192021" } };
    InlineKeyboardMarkup roulette = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("PP") } });
    public Dictionary<string, string> totalwin = new Dictionary<string, string>();
    private Dictionary<string, DateTime> rolls = new Dictionary<string, DateTime>();
    private string namenow;
    private string unamenow;
    private bool online = false;
    private int keyboard = 0;
    private DateTime timeMade; 
    public Roulette()
    {
    }

    private async void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      if (e.CallbackQuery.Message.Text != "Roulette") return;
      keyboard = e.CallbackQuery.Message.MessageId;
      if (e.CallbackQuery.Data == "Крутить" && status != true)
      {
        namenow = e.CallbackQuery.From.FirstName;
        unamenow = e.CallbackQuery.From.Username;
        await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Удачи!");
        id1 = e.CallbackQuery.Message.Chat.Id;
        id2 = e.CallbackQuery.Message.MessageId;
        CanRoll();
      }
      else if (status == true) await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Сейчас крутит {namenow}");
      else await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Не хитри! Нажми кнопку <Крутить>");
    }

   /* public void CheckRoll()
    {
      if (online == true) return;
      if (DateTime.Now.Subtract(timeMade).TotalMinutes < 3)
    }
*/
    public void CreateRoll()
    {
      Bot.OnCallbackQuery += bot_OnCallbackQuery;
      var message = m.Message;
      roulette = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Рапира") // 50%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Короткий меча") // 5%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Сабля") // 30%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("3 💰") // 15%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("🎰Крутить🎰", "Крутить")
        }
      });
      Bot.SendTextMessageAsync(message.From.Id, "Roulette", replyMarkup: roulette);
      online = true;
    }

    private async void Roll(InlineKeyboardMarkup roulette1)
    {
      LoadTotalWin();
      if (status == true) return;
      status = true;
      int x = 1;
      int win = 0;
      Random rnd = new Random();
      int rn = rnd.Next(1, 21);

      for (int i = 0; i < presents.Count(); i++)
      {
        for (int j = 0; j < presents.ElementAt(i).Value.Length; j += 2)
        {
          string t = presents.ElementAt(i).Value.Substring(j, 2);
          if (t.Substring(0, 1) == "0") t = t.Remove(0,1);
          if(t == rn.ToString())
          {
            win = i;
            break;
          }
        }
      }
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 4; j++)
        {
          if (!roulette1.InlineKeyboard.ElementAt(0).ElementAt(0).Text.Contains("⬅️"))
          {
            roulette1.InlineKeyboard.ElementAt(j).ElementAt(0).Text = roulette1.InlineKeyboard.ElementAt(j).ElementAt(0).Text + "⬅️";
            await Bot.EditMessageTextAsync(id1, id2, "Roulette", replyMarkup: roulette1);
          }
          Thread.Sleep(500 * x);
          roulette1.InlineKeyboard.ElementAt(j).ElementAt(0).Text = roulette1.InlineKeyboard.ElementAt(j).ElementAt(0).Text.Replace("⬅️", "");
        }
        x++;
      }
      for (int i = 0; i <= win; i++)
      {
        roulette1.InlineKeyboard.ElementAt(i).ElementAt(0).Text = roulette1.InlineKeyboard.ElementAt(i).ElementAt(0).Text + "⬅️";
        await Bot.EditMessageTextAsync(id1, id2, "Roulette", replyMarkup: roulette1);
        Thread.Sleep(200 * x);
        roulette1.InlineKeyboard.ElementAt(i).ElementAt(0).Text = roulette1.InlineKeyboard.ElementAt(i).ElementAt(0).Text.Replace("⬅️", "");
      }
      AddPresent(unamenow, win);
      await Bot.SendTextMessageAsync(m.Message.Chat.Id, $"@{unamenow} выиграл {presents.ElementAt(win).Key}, поздравляю!");
      Thread.Sleep(3000);
      await Bot.EditMessageTextAsync(id1, id2, "Roulette", replyMarkup: roulette);
      status = false;
    }

    public void Inventory()
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

    private void CanRoll()
    {
      if (status == true) return;
      if (rolls.ContainsKey(unamenow))
      {
        if (rolls[unamenow].Subtract(DateTime.Now).TotalMilliseconds > 30)
        {
          Roll(roulette);
          rolls[unamenow] = DateTime.Now;
        }
        else
        {
          Bot.SendTextMessageAsync(m.Message.From.Id, "Крутить можно раз в 2 часа :3");
          return;
        }
      }
      else
      {
        rolls.Add(unamenow, DateTime.Now);
        Roll(roulette);
      }
    }

    private void AddPresent(string name, int winId)
    {
      if (totalwin.ContainsKey(name))
      {
        if (totalwin[name].Contains("💰") && presents.ElementAt(winId).Key.Contains("💰"))
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
        else if (presents.ElementAt(winId).Key.Contains("💰") && !totalwin[name].Contains("💰"))
        {
          string t = totalwin[name];
          totalwin[name] = t + ":" + presents.ElementAt(winId).Key;
        }
        else if (totalwin[name].Contains(presents.ElementAt(winId).Key.Substring(0, 5)) && presents.ElementAt(winId).Key.Contains("x"))
        {
          int index = totalwin[name].IndexOf("x") + 1;
          string test = presents.ElementAt(winId).Key.Substring(0, 3);
          if (totalwin[name].IndexOf(test) > index)
          {
            string ntest = presents.ElementAt(winId).Key;
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
        else if (presents.ElementAt(winId).Key.Contains("x") && !totalwin[name].Contains(presents.ElementAt(winId).Key.Substring(0, 5)))
        {
          string t = totalwin[name];
          totalwin[name] = t + ":" + presents.ElementAt(winId).Key;
        }

      }
      else totalwin.Add(unamenow, $":{presents.ElementAt(winId).Key}");
      SaveTotalWin();
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