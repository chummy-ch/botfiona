﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;

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
      hp.Add(p2, 10);
    }

    public void PreStart()
    {
      bot.OnCallbackQuery += bot_OnCallbackQuery;
      LoadWins();
      Start();
    }
    
    public  void Start()
    {
      var message = e.Message;/*
      if (DateTime.Now.Subtract(Program.w8).TotalSeconds > 62)
      {
        FinishBattle();
        return;
      }*/
      x = 0;
      Thread.Sleep(1000);
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
      bot.SendTextMessageAsync(message.Chat.Id, $"Раунд № {round} ✨ ");
      Thread.Sleep(1000);
      bot.SendTextMessageAsync(message.Chat.Id, "Атака", replyMarkup: choice);
      round++;
    }

    private void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      Program.w8 = DateTime.Now;
      var choice = e.CallbackQuery.Message.ReplyMarkup;
      index = e.CallbackQuery.Message.MessageId;
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
      }
      else if (x == 1)
      {
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
          Thread.Sleep(500);
          bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, index);
          BattleRes(act1, def2, p2) ;
          BattleRes(act2, def1, p1) ;
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


    private void BattleRes(string attack, string deffend, string UNameDeffender)
    {
      act1 = "";
      def1 = "";
      act2 = "";
      def2 = "";
      Thread.Sleep(1000);
      if (deffend != attack)
      {
        hp[UNameDeffender] -= atdef[attack];
        if (hp[UNameDeffender] < 0) hp[UNameDeffender] = 0;
        Thread.Sleep(700);
        bot.SendTextMessageAsync(e.Message.Chat.Id, CreatMessage(attack, UNameDeffender));
      }
      if (hp[UNameDeffender] <= 0) hp[UNameDeffender] = 0;
      Thread.Sleep(700);
      bot.SendTextMessageAsync(e.Message.Chat.Id, $" У @{UNameDeffender}  {hp[UNameDeffender]} ❤️");
      if (hp[UNameDeffender] <= 0)
      {
        Thread.Sleep(1000);
        bot.DeleteMessageAsync(e.Message.Chat.Id, index);
        FinishBattle();
      }
      else Start();
    }

    private string CreatMessage(string attack,  string UNameDeffender)
    {
      string mes = "";
      string UNameAttacker;
      if (hp.ElementAt(0).Key != UNameDeffender)  UNameAttacker = hp.ElementAt(0).Key;
      else  UNameAttacker = hp.ElementAt(1).Key;
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

    public async void FinishBattle()
    {
      Thread.Sleep(3000);
      if (hp.Count == 2 && hp[p1] <= 0 && hp[p2] <= 0)
      {
        await bot.SendTextMessageAsync(e.Message.Chat.Id, "Ничья!");
      }
      else if (hp.Count == 2)
      {
        string winner;
        if (hp[p1] <= 0) winner =  p2;
        else winner = p1;
        await bot.SendTextMessageAsync(e.Message.Chat.Id, $"@{winner} победил в этом бою!");
        await bot.SendStickerAsync(e.Message.Chat.Id, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
        if (pwins.ContainsKey(winner)) pwins[winner] += 1;
        else pwins.Add(winner, 1);
        SaveWins();
      }
      Program.online = false;
      hp = new Dictionary<string, int>();
      await bot.SendTextMessageAsync(e.Message.Chat.Id, "Бой завершен.");
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
