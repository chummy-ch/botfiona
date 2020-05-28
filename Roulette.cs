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
using Bot_Fiona;
using System.Runtime.InteropServices.WindowsRuntime;
using Telegram.Bot.Types;

namespace botfiona
{
  class Roulette
  {
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;
    private long id1;
    private int id2;
    private bool status = false;
    
    public Dictionary<string, string> presents = new Dictionary<string, string>() { { "Рапира", "5" }, { "Меч", "70" }, { "Сабля", "25" }, { "Щит", "70" }, {"ничего", "30" } };
    InlineKeyboardMarkup roulette = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("PP") } });
    private Dictionary<string, DateTime> rolls = new Dictionary<string, DateTime>();
    private string namenow;
    private string unamenow;
    private bool online = false;
    private int keyboard = 0;
    private DateTime timeMade; 
    public Roulette()
    {
      LoadRollsTime();
    }

    private async void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      if (e.CallbackQuery.Message.Text != "Roulette") return;
      keyboard = e.CallbackQuery.Message.MessageId;
      if (e.CallbackQuery.Data == "Крутить" && status != true)
      {
        namenow = e.CallbackQuery.From.FirstName;
        unamenow = e.CallbackQuery.From.Username;
        id1 = e.CallbackQuery.Message.Chat.Id;
        id2 = e.CallbackQuery.Message.MessageId;
        await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Удачи!");
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
          InlineKeyboardButton.WithCallbackData("Меч") // 5%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Сабля") // 30%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Щит") // 15%
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Крендель")
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("🎰Крутить🎰", "Крутить")
        }
      });
      keyboard = Bot.SendTextMessageAsync(message.From.Id, "Roulette", replyMarkup: roulette).Result.MessageId;
      online = true;
    }

    private async void Roll(InlineKeyboardMarkup roulette1)
    {
      if (status == true) return;
      status = true;
      int x = 1;
      int win = 0;
      Random rnd = new Random();
      int rn = rnd.Next(1, 100);

      if (rn <= 5) win = 0;
      else if (rn <= 25) win = 2;
      else if (rn <= 70)
      {
        rn = rnd.Next(1, 100); ;
        if (rn <= 50) win = 3;
        else win = 1;
      }
      else win = 4;


      for (int i = 0; i < 2; i++)
      {
        for (int j = 0; j < 5; j++)
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
      if (win == 4)
      {
       await Bot.SendTextMessageAsync(m.Message.Chat.Id, "Приходите в следующий раз");
        return;
      }
      Inventory invent = new Inventory();
      invent.AddPresent(unamenow, win, unamenow);
      await Bot.SendTextMessageAsync(m.Message.Chat.Id, $"@{unamenow} выиграл {presents.ElementAt(win).Key}, поздравляю!");
      Thread.Sleep(3000);
      await Bot.EditMessageTextAsync(id1, id2, "Roulette", replyMarkup: roulette);
      status = false;
      await Bot.DeleteMessageAsync(m.Message.From.Id, keyboard);
    }
   
    private async void CanRoll()
    {
      if (status == true) return;
      if (rolls.ContainsKey(unamenow))
      {
        if (DateTime.Now.Subtract(rolls[unamenow]).TotalHours >= 8)
        {
          Roll(roulette);
          rolls[unamenow] = DateTime.Now;
        }
        else
        {
          await Bot.SendTextMessageAsync(m.Message.From.Id, "Крутить можно раз в 8 часов :3");
          await Bot.DeleteMessageAsync(m.Message.From.Id, keyboard);
          return;
        }
      }
      else
      {
        rolls.Add(unamenow, DateTime.Now);
        Roll(roulette);
      }
      SaveRollsTime();
    }


    private void SaveRollsTime()
    {
      using (StreamWriter writer = System.IO.File.CreateText("RollsTime.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, rolls);
      }
    }

    private void LoadRollsTime()
    {
      if (!System.IO.File.Exists("RollsTime.txt")) return;
      string json = System.IO.File.ReadAllText("RollsTime.txt");
      rolls = new JavaScriptSerializer().Deserialize<Dictionary<string, DateTime>>(json);
    }


  }
}