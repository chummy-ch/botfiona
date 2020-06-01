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
using System.Net.Http.Headers;
using System.Web.UI.WebControls;
using System.Security.Cryptography;

namespace botfiona
{
  public class Battle
  {
    private static TelegramBotClient bot;
    private MessageEventArgs e;
    public static Dictionary<string, int> pwins = new Dictionary<string, int>();
    private string p1, p2;
    protected static Dictionary<string, int> hp = new Dictionary<string, int>();
    private string act1 = "", act2 = "";
    private string def1 = "", def2 = "";
    readonly string at = "⚔️";
    readonly string def = " 🛡";
    private int x;
    private static long chatid;
    private static int finishBAttleCheck = 0;
    private static int freshBoard;
    private int round = 1;
    private int prestartOnline = 0;
    private TimerCallback tm;
    private Timer timer;
    private int w1, w2;
    private Dictionary<string, int> atdef = new Dictionary<string, int>()
    
    {
      { "Голову", 3}, {"Туловище", 2}, {"Ноги", 1}
    };
    public Battle(TelegramBotClient bot, MessageEventArgs e)
    {
      this.e = e;
      Battle.bot = bot;
    }

    public Battle()
    {

    }

    public void SetFirstPlayer(string p)
    {
      p1 = p;
      hp.Add(p1, 15);
      w1 = GetWeapon(p);
    }

    public void SetSecondPlayer(string p)
    {
      p2 = p;
      if (hp.Count < 2) 
      hp.Add(p2, 15);
      w2 = GetWeapon(p);
    }

    private int GetWeapon(string uname)
    {
      Inventory inv = new Inventory();
      string weapon = "";
      if (inv.weapon.ContainsKey(uname)) weapon = inv.weapon[uname];
      if (weapon.Length > 0)
      {
        Weapon weap = new Weapon();
        return weap.weaponDamage[weapon];
      }
      else return 0;
    }

    public void PreStart()
    {
      if (prestartOnline != 0) return;
      prestartOnline++;
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
      string dash = "- - - - - - - - - - - - - - - - - - - - - - - - - - -\n";
      Thread.Sleep(300);
      bot.SendTextMessageAsync(message.Chat.Id, $"{dash}Раунд № {round} ✨ "); 
      Thread.Sleep(800);
      var result = bot.SendTextMessageAsync(message.Chat.Id, "Атака", replyMarkup: choice).Result;
      freshBoard = result.MessageId;
      chatid = result.Chat.Id;
      round++;
    }

    private void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      if (e.CallbackQuery.Message.MessageId != freshBoard) return;
      if (!hp.ContainsKey(e.CallbackQuery.From.Username))
      {
        bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Ты не участвуешь");
        return;
      }
      var choice = e.CallbackQuery.Message.ReplyMarkup;
      string c = e.CallbackQuery.Data;
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
          bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, freshBoard);
          BattleRes(act1, def2, p2);
          BattleRes(act2, def1, p1);
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
    
    private void  BattleRes(string attack, string deffend, string UNameDeffender)
    {
      if (hp.Count == 0) return;
      int wA;
      int wD;
      if(UNameDeffender == p1)
      {
        wA = w2;
        wD = w1;
      }
      else
      {
        wA = w1;
        wD = w2;
      }
      if (deffend != attack)
      {
        int plusDamage = 0;
        if(wA > 0)
        {
          Random rn = new Random();
          if(rn.Next(1,100) <= 30 )
          {
            plusDamage = wA; 
          }
        }
        if(wD < 0)
        {
          Random rn = new Random();
          if(rn.Next(1,100) <= 30)
          {
            plusDamage += wD;
          }
        }
        hp[UNameDeffender] -= (atdef[attack] + plusDamage);
        if (hp[UNameDeffender] < 0) hp[UNameDeffender] = 0;
        Thread.Sleep(1000);
        bot.SendTextMessageAsync(e.Message.Chat.Id, CreatMessage(attack, UNameDeffender, plusDamage));
        Thread.Sleep(350);
      }
      else
      {
        if (wA == 3)
        {
          Random rn = new Random();
          if (rn.Next(1, 100) <= 30)
          {
            hp[UNameDeffender] -= 1;
            bot.SendTextMessageAsync(e.Message.Chat.Id, $"{UNameDeffender} получает крит в 1 ❤️");
          }
        }
      }
      if (hp[UNameDeffender] < 0) hp[UNameDeffender] = 0;
      Thread.Sleep(1000);
      bot.SendTextMessageAsync(e.Message.Chat.Id, $" У {UNameDeffender}  {hp[UNameDeffender]} ❤️");
      if (hp[UNameDeffender] <= 0)
      {
        Thread.Sleep(450);
        bot.DeleteMessageAsync(e.Message.Chat.Id, freshBoard);
        freshBoard = 0;
        FinishBattle();
        return;
      }
    }

    private string CreatMessage(string attack, string UNameDeffender, int plusDamage)
    {
      string krit = "";
      if (plusDamage != 0) krit = "⚡️";
      string mes = "";
      string UNameAttacker;
      if(hp.Count == 0) return "конец боя";
      if (hp.ElementAt(0).Key != UNameDeffender) UNameAttacker = hp.ElementAt(0).Key;
      else UNameAttacker = hp.ElementAt(1).Key;
      Random rnd = new Random();
      int rn = rnd.Next(0, 5);
      if (hp[UNameDeffender] < 0) hp[UNameDeffender] = 0;
      switch (rn)
      {
        case 1:
          mes = $"@{UNameDeffender} пропустил удар в {krit}{attack} ";
          break;
        case 3:
          mes = $"@{UNameDeffender} не защититил {krit}{attack} и потерял {atdef[attack] + plusDamage} hp";
          break;
        case 2:
          mes = $"@{UNameAttacker} пробил защиту противника и нанес удар в {krit}{attack}";
          break;
        case 4:
          mes = $"@{UNameAttacker} обманом ударил @{UNameDeffender} в {krit}{attack} и отнял {atdef[attack] + plusDamage} hp";
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

    private void TimerFinish(object obj)
    {
      FinishBattle();
    }

    public  void FinishBattle()
    {
      finishBAttleCheck++;
      TimerCallback tm = new TimerCallback(TimerFinish);
      Timer timer = new Timer(tm, 0, 4000, 0);
      if (finishBAttleCheck < 2) return;
      timer.Dispose();
      Thread.Sleep(300);
      try
      {
        if (hp.Count > 0 && hp[p1] <= 0 && hp[p2] <= 0)
        {
          bot.SendTextMessageAsync(e.Message.Chat.Id, "Ничья!");
          Thread.Sleep(300);
          bot.SendStickerAsync(e.Message.Chat.Id, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
        }
        else
        {
          string winner;
          if (hp[p1] <= 0) winner = p2;
          else winner = p1;
          bot.SendTextMessageAsync(e.Message.Chat.Id, $"@{winner} победил в этом бою!");
          Thread.Sleep(300);
          bot.SendStickerAsync(e.Message.Chat.Id, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
          if (pwins.ContainsKey(winner)) pwins[winner] += 1;
          else pwins.Add(winner, 1);
          SaveWins();
        }
      }
      catch
      {
        hp.Clear();
        BattleManager.online = false;
      }
      hp.Clear();
      BattleManager.online = false;
    }

    public static void StopBattle()
    {
      BattleManager.online = false;
      hp.Clear();
      finishBAttleCheck++;
      bot.DeleteMessageAsync(chatid, freshBoard);
      bot.SendTextMessageAsync(chatid, "Бой становлен");
      freshBoard = 0;
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
  