using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;
using Bot_Fiona;

namespace botfiona
{
  public class Battle
  {
    private TelegramBotClient bot;
    private MessageEventArgs e;
    public static Dictionary<string, int> pwins = new Dictionary<string, int>();
    private string p1 = "", p2 = "";
    private Dictionary<string, int> hp = new Dictionary<string, int>();
    private string act1 = "", act2 = "";
    private string def1 = "", def2 = "";
    readonly string at = "⚔️";
    readonly string def = " 🛡";
    private int x;
    private int index = 0;
    private int finishBAttleCheck = 0;
    private int freshCallBack = 0;
    private int round = 1;
    private Dictionary<string, int> atdef = new Dictionary<string, int>()
    
    {
      { "Голову", 3}, {"Туловище", 2}, {"Ноги", 1}
    };
    public Battle(TelegramBotClient bot, MessageEventArgs e)
    {
      this.e = e;
      this.bot = bot;
    }

    public Battle()
    {

    }
    public void SetFirstPlayer(string p)
    {
      p1 = p;
      hp.Add(p1, 10);
    }

    public void SetSecondPlayer(string p)
    {
      p2 = p;
      if (hp.Count < 2) 
      hp.Add(p2, 10);
    }

    public void PreStart()
    {
      bot.OnCallbackQuery += bot_OnCallbackQuery;
      LoadWins();
      Start();
    }

    public void Start()
    {
      var message = e.Message;
      /*if (DateTime.Now.Subtract(Program.w8).TotalSeconds > 62)
      {
        StopBattle();
        return;
      }*/
      if (hp.Count != 2 || hp[p1] == 0 || hp[p2] == 0) return;
      x = 0;
      Thread.Sleep(100);
      InlineKeyboardMarkup choice = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Голова " + at, "Голову")
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Туловище " + at, "Туловище")
        },
        new[]
        {
          InlineKeyboardButton.WithCallbackData("Ноги " + at, "Ноги")
        }
      });
      Thread.Sleep(1000);
      bot.SendTextMessageAsync(message.Chat.Id, $"Раунд № {round} ✨ ");
      Thread.Sleep(800);
      freshCallBack = bot.SendTextMessageAsync(message.Chat.Id, "Атака", replyMarkup: choice).Result.MessageId;
      round++;
    }

    private void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      if (e.CallbackQuery.Message.MessageId != freshCallBack) return;
      var choice = e.CallbackQuery.Message.ReplyMarkup;
      string c = e.CallbackQuery.Data;
      index = e.CallbackQuery.Message.MessageId;
      Thread.Sleep(700);
      if (x == 0)
      {
        if (e.CallbackQuery.From.Username == p1)
        {
          act1 = e.CallbackQuery.Data;
        }
        else if (e.CallbackQuery.From.Username == p2)
        {
          act2 = e.CallbackQuery.Data;
        }
        bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы выбрали {e.CallbackQuery.Data}");
        if (act1.Length > 0 && act2.Length > 0)
        {
          bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, "Защита", replyMarkup: KeyboatdToDeffend(choice));
        }
        Thread.Sleep(300);
      }
      else if (x == 1)
      {
        Thread.Sleep(300);
        if (e.CallbackQuery.From.Username == p1)
        {
          def1 = e.CallbackQuery.Data;
        }
        else if (e.CallbackQuery.From.Username == p2)
        {
          def2 = e.CallbackQuery.Data;
        }
        bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы выбрали {e.CallbackQuery.Data}");
        if (def1.Length > 0 && def2.Length > 0)
        {
          Thread.Sleep(600);
          bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, index);
          int z = BattleRes(act1, def2, p2);
          z = BattleRes(act2, def1, p1) + 1;
          act1 = "";
          def1 = act1;
          act2 = act1;
          def2 = act1;
          Thread.Sleep(800);
          Start();
        }
      }
    }


    private InlineKeyboardMarkup KeyboatdToDeffend(InlineKeyboardMarkup choice)
    {
      for (int i = 0; i < 3; i++)
      {
        choice.InlineKeyboard.ElementAt(i).ElementAt(0).Text = atdef.ElementAt(i).Key + def;
      }
      x += 1;
      return choice;
    }


    private int BattleRes(string attack, string deffend, string UNameDeffender)
    {
      if (hp.Count == 0) return 0;
      if (deffend != attack)
      {
        hp[UNameDeffender] -= atdef[attack];
        if (hp[UNameDeffender] < 0) hp[UNameDeffender] = 0;
        Thread.Sleep(1000);
        bot.SendTextMessageAsync(e.Message.Chat.Id, CreatMessage(attack, UNameDeffender));
        Thread.Sleep(350);
      }
      if (hp[UNameDeffender] < 0) hp[UNameDeffender] = 0;
      Thread.Sleep(1000);
      bot.SendTextMessageAsync(e.Message.Chat.Id, $" У {UNameDeffender}  {hp[UNameDeffender]} ❤️");
      if (hp[UNameDeffender] <= 0)
      {
        Thread.Sleep(450);
        bot.DeleteMessageAsync(e.Message.Chat.Id, index);
        index = 0;
        FinishBattle();
        return 0;
      }
      else return 0;
    }

    private string CreatMessage(string attack, string UNameDeffender)
    {
      string mes = "";
      string UNameAttacker;
      if(hp.Count == 0) return "конец боя";
      if (hp.ElementAt(0).Key != UNameDeffender) UNameAttacker = hp.ElementAt(0).Key;
      else UNameAttacker = hp.ElementAt(1).Key;
      Random rnd = new Random();
      int rn = rnd.Next(1, 4);
      if (hp[UNameDeffender] < 0) hp[UNameDeffender] = 0;
      switch (rn)
      {
        case 1:
          mes = $"@{UNameDeffender} пропустил удар в {attack} ";
          break;
        case 3:
          mes = $"@{UNameDeffender} не защититил {attack} и потерял {atdef[attack]} hp";
          break;
        case 2:
          mes = $"@{UNameAttacker} пробил защиту противника и нанес удар в {attack}";
          break;
        case 4:
          mes = $"@{UNameAttacker} обманом ударил @{UNameDeffender} в {attack} и отнял {atdef[attack]} hp";
          break;
      }
      Thread.Sleep(250);
      return mes;
    }

    public static int GetWins(string UserName)
    {
      LoadWins();
      if (pwins.ContainsKey(UserName)) return pwins[UserName];
      else return 0;
    }

    public  void FinishBattle()
    {
      finishBAttleCheck++;
      if (finishBAttleCheck != 2) return;
      BattleManager.online = false;
      Thread.Sleep(300);
      if (hp[p1] <= 0 && hp[p2] <= 0)
      {
        bot.SendTextMessageAsync(e.Message.Chat.Id, "Ничья!");
        bot.SendStickerAsync(e.Message.Chat.Id, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
      }
      else
      {
        string winner;
        if (hp[p1] <= 0) winner = p2;
        else winner = p1;
         bot.SendTextMessageAsync(e.Message.Chat.Id, $"@{winner} победил в этом бою!");
         bot.SendStickerAsync(e.Message.Chat.Id, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
        if (pwins.ContainsKey(winner)) pwins[winner] += 1;
        else pwins.Add(winner, 1);
        SaveWins();
      }
      hp.Clear();
    }

    public async void StopBattle()
    {
      BattleManager.online = false;
      hp = new Dictionary<string, int>();
      if (index != 0) await bot.DeleteMessageAsync(e.Message.Chat.Id, index);
      await bot.SendTextMessageAsync(Program.chatid, "Бой остановлен.");
      await bot.DeleteMessageAsync(e.Message.Chat.Id, BattleManager.index1);
      return;
    }

    static void SaveWins()
    {
      using (StreamWriter writer = File.CreateText("wins.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, pwins);
      }
    }

    public static void LoadWins()
    {
      if (!File.Exists("wins.txt")) return;
      string json = File.ReadAllText("wins.txt");
      pwins = new JavaScriptSerializer().Deserialize<Dictionary<string, int>>(json);
    }
  }


}
