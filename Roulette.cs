using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
namespace botfiona
{
  class Roulette
  {
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;
    private long id1;
    private int id2;
    private Dictionary<string, string> presents = new Dictionary<string, string>() { { "3 💰", "2,3,6,8,10,11,14,16,18,19,20,21" }, { "Рапира", "1" }, { "Короткий меч", "4,7,9,13,15" }, { "Длинный меч", "5,12,17" } };
    InlineKeyboardMarkup roulette = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("PP") } });
    public Roulette()
    {

    }

    private async void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      if (e.CallbackQuery.Data == "Крутить")
      {
        await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Удачи!");
        id1 = e.CallbackQuery.Message.Chat.Id;
        id2 = e.CallbackQuery.Message.MessageId;
        Roll(roulette);
      }
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
      int x = 1;
      int win = 0;
      Random rnd = new Random();
      int rn = rnd.Next(1, 21);
      for (int i = 0; i < presents.Count(); i++)
      {
        if (presents.ElementAt(i).Value.Contains(rn.ToString()))
        {
          win = i ;
          Console.WriteLine(presents.ElementAt(i).Key);
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
      await Bot.EditMessageTextAsync(id1, id2, "Roulette", replyMarkup: roulette);
      await Bot.SendTextMessageAsync(m.Message.Chat.Id, $"Твой приз - {presents.ElementAt(win).Key}");
    }
  }
}