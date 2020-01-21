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
    private Dictionary<string, int> hp = new Dictionary<string, int>();
    private string act1 = "", act2 = "";
    private string def1 = "", def2 = "";
    string at = "⚔️";
    string def = " 🛡";
    private int x = 0;
    private int index = 0;
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
      hp.Add(p1, 3);
    }

    public void SetSecondPlayer(string p)
    {
      p2 = p;
      hp.Add(p2, 3);
    }

    public void PreStart()
    {
      bot.OnCallbackQuery += bot_OnCallbackQuery;
      Start();
    }
    
    public  void Start()
    {
      Console.WriteLine("Start");
      var message = e.Message;
      if (hp[p1] == 0 || hp[p2] == 0) return;

      InlineKeyboardMarkup choice = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Голова " + at, "Голова")
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Туловище " + at, "Тело")
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Ноги " + at, "Ноги")
        }
      });
      x = 0;
       bot.SendTextMessageAsync(message.Chat.Id, "Атака", replyMarkup: choice);
    }

    private async void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      var choice = e.CallbackQuery.Message.ReplyMarkup;
      string c = e.CallbackQuery.Data;
      index = e.CallbackQuery.Message.MessageId;
      Console.WriteLine("Вход");
      if (x == 0)
      {
        Console.WriteLine("x = 0");
        if (e.CallbackQuery.From.Username == p1)
        {
          act1 = e.CallbackQuery.Data;
        }
        else if (e.CallbackQuery.From.Username == p2)
        {
          act2 = e.CallbackQuery.Data;
        }
        await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы выбрали {e.CallbackQuery.Data}");
        if (act1.Length > 0 && act2.Length > 0)
        {
          await bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, "Защита", replyMarkup: KeyboatdToDeffend(choice));
        }
      }
      else
      {
        if (e.CallbackQuery.From.Username == p1)
        {
          def1 = e.CallbackQuery.Data;
        }
        else if (e.CallbackQuery.From.Username == p2)
        {
          def2 = e.CallbackQuery.Data;
        }
        await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы выбрали {e.CallbackQuery.Data}");
        if (def1.Length > 0 && def2.Length > 0)
        {
          await bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, index);
          int z = BattleRes(act1, def2, p2) ;
          z = BattleRes(act2, def1, p1) + 1;
          Console.WriteLine(z);
          act1 = "";
          def1 = act1;
          act2 = act1;
          def2 = act1;
          Start();
        }
      }
    }


    /*private InlineKeyboardMarkup KeyboatdToAttack(InlineKeyboardMarkup choice, int InlineMessageId)
    {
      for (int i = 0; i < 3; i++)
      {
        choice.InlineKeyboard.ElementAt(i).ElementAt(0).Text = atdef.ElementAt(i).Key + at;
      }
      x = 0;
      return choice;
    }*/

    private InlineKeyboardMarkup KeyboatdToDeffend(InlineKeyboardMarkup choice)
    {
      for (int i = 0; i < 3; i++)
      {
        choice.InlineKeyboard.ElementAt(i).ElementAt(0).Text = atdef.ElementAt(i).Key + def;
      }
      x = 1;
      return choice;
    }


    private int BattleRes(string attack, string deffend, string UNameDeffender)
    {
      if (deffend != attack) hp[UNameDeffender] -= 1;
       bot.SendTextMessageAsync(e.Message.Chat.Id, $"{UNameDeffender} = {hp[UNameDeffender]}");
      if (hp[UNameDeffender] == 0)
      {
        bot.DeleteMessageAsync(e.Message.Chat.Id, index);
        FinishBattle();
        return 0;
      }
      else return 0;
    }
    private  void FinishBattle()
    {
      string winner = "";
      if (hp[p1] > 0) winner = "@" + p1;
      else winner = "@" + p2;
       bot.SendTextMessageAsync(e.Message.Chat.Id, $"{winner} победил в этом бою!");
       bot.SendStickerAsync(e.Message.Chat.Id, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
    }
  }
}
