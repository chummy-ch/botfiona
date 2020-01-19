using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace botfiona
{
  public class Battle
  {
    private TelegramBotClient bot;
    private MessageEventArgs e;
    private string p1 = "", p2 = "";
    private Dictionary<string, double> atdef = new Dictionary<string, double>()
    {
      { "Голова", 1.9 }, {"Туловище", 1.5}, {"Ноги", 0.9}
    };
    public Battle(TelegramBotClient bot, MessageEventArgs e)
    {
      this.e = e;
      this.bot = bot;

    }

    public void SetFirstPlayer(string p)
    {
      p1 = p;
    }

    public void SetSecondPlayer(string p)
    {
      p2 = p;
    }

    public async void Start()
    {
      var message = e.Message;
      var choice = new ReplyKeyboardMarkup (new[]
    {
        new[]
        {
          new KeyboardButton(atdef.ElementAt(0).Key),
          new KeyboardButton(atdef.ElementAt(1).Key),
          new KeyboardButton(atdef.ElementAt(2).Key)
        }
      });
      await bot.SendTextMessageAsync(message.Chat.Id, "Сделайте выбор", replyMarkup: choice);

      
    }

    async void BattleDur(string fc, string sc)
    {
      var message = e.Message;
     
    }

  }
}
