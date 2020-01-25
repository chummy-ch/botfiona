using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    private Dictionary<string, string> presents = new Dictionary<string, string>() { { "3 💰", "2,3,6,8,10,11,14,16,18,19,20,21" }, { "Рапира", "1" }, { "Короткий меч", "4,7,9,13,15" }, { "Длинный меч", "5,12,17" } };
    InlineKeyboardMarkup roulette = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("PP") } });
    public Dictionary<string, string> totalwin = new Dictionary<string, string>();
    private Dictionary<string, DateTime> rolls = new Dictionary<string, DateTime>();
    private string namenow;
    private string unamenow;
    public Roulette()
    {

    }

    private async void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
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

    public void CreateRoul()
    {
      Bot.OnCallbackQuery += bot_OnCallbackQuery;
      var message = m.Message;
      roulette = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithCallbackData("3 💰") // 50%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Рапира") // 5%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Короткий меч") // 30%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Длинный меч") // 15%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("🎰Крутить🎰", "Крутить")
        }
      });
      Bot.SendTextMessageAsync(message.Chat.Id, "Roulette", replyMarkup: roulette);
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
        if (presents.ElementAt(i).Value.Contains(rn.ToString()))
        {
          win = i ;
          break;
        }
      }
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 4; j++)
        {
          if(!roulette1.InlineKeyboard.ElementAt(0).ElementAt(0).Text.Contains("⬅️"))
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
        Thread.Sleep(200 * x );
        roulette1.InlineKeyboard.ElementAt(i).ElementAt(0).Text = roulette1.InlineKeyboard.ElementAt(i).ElementAt(0).Text.Replace("⬅️", "");
      }
      await Bot.SendTextMessageAsync(m.Message.Chat.Id, $"@{unamenow} выиграл {presents.ElementAt(win).Key}, поздравляю!");
      if (totalwin.ContainsKey(unamenow))
      {
        string t = totalwin[unamenow] + $":{presents.ElementAt(win).Key}:";
        totalwin[unamenow] = t;
      }
      else totalwin.Add(unamenow, $":{presents.ElementAt(win).Key}:");
      SaveTotalWin();
      Thread.Sleep(3000);
      await Bot.EditMessageTextAsync(id1, id2, "Roulette", replyMarkup: roulette);
      status = false;
    }

    public void Presents()
    {
      LoadTotalWin();
      Console.WriteLine(1);
      string uName = m.Message.From.Username;
      if (!totalwin.ContainsKey(uName)) return;
      Console.WriteLine(2);
      string[] arr = totalwin[uName].Split(':');
      string t = "Призы за все время:";
      for(int i = 0; i < arr.Length; i++)
      {
        t += $"\n{arr[i]}";
      }
      Bot.SendTextMessageAsync(m.Message.Chat.Id, t);
    }
    private void CanRoll()
    {
      if (status == true) return;
      if (rolls.ContainsKey(unamenow))
      {
        if (rolls[unamenow].Subtract(DateTime.Now).TotalMilliseconds >= 120)
        {
          Console.WriteLine(1);
          Roll(roulette);
          rolls[unamenow] = DateTime.Now;
        }
        else Bot.SendTextMessageAsync(m.Message.From.Id, "Крутить можно раз в 2 часа :3");
      }
      else
      {
        rolls.Add(unamenow, DateTime.Now);
        Roll(roulette);
      }
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