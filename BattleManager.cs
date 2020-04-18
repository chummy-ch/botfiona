﻿using botfiona;
using System;
using System.Threading;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot_Fiona
{
  public class BattleManager
  {
    private MessageEventArgs e;
    static public bool online = false;
    public TelegramBotClient Bot;
    static Battle battle;
    private long chatId;
    private int freshBoard;
    private TimerCallback tm;
    private Timer timer;


    public BattleManager(MessageEventArgs e)
    {
      this.e = e;
      this.Bot = Program.Bot;
      battle = new Battle(Bot, e);
      Bot.OnCallbackQuery += Bot_OnCallbackQuery;
    }


    public void PreBattle()
    {
      tm = new TimerCallback(ChechTime);
      timer = new Timer(tm, 0, 10000, 0);
      var message = e.Message;
      if (message.From.Username != null && online == false/* && message.Chat.Title.Contains("arena")*/)
      {
        long chatid = message.Chat.Id;
        var w8 = DateTime.Now;
        online = true;
        battle.SetFirstPlayer(message.From.Username);
        InlineKeyboardMarkup markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
              InlineKeyboardButton.WithCallbackData(message.From.FirstName),
              InlineKeyboardButton.WithCallbackData("Второй боец")
            }

          });
        var result = Bot.SendTextMessageAsync(message.Chat.Id, "Великая битва!", replyMarkup: markup).Result;
        freshBoard = result.MessageId;
        chatId = result.Chat.Id;
      }

    }

    private void ChechTime(object obj)
    {
      Bot.DeleteMessageAsync(chatId, freshBoard);
      freshBoard = 0;
      online = false;
      timer.Dispose();
    }

    private async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      if (e.CallbackQuery.Message.MessageId != freshBoard) return;
      timer.Dispose();
      var keyboard = e.CallbackQuery.Message.ReplyMarkup;
      if (e.CallbackQuery.From.FirstName == keyboard.InlineKeyboard.ElementAt(0).ElementAt(0).Text)
      {
        await Program.Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Ты и так участвуешь, чучело...");
        return;
      }
      if (e.CallbackQuery.Data.Equals("Второй боец") && battle != null)
      {
        battle.SetSecondPlayer(e.CallbackQuery.From.Username);
        //secondp = e.CallbackQuery.From.Username;
        keyboard.InlineKeyboard.ElementAt(0).ElementAt(1).Text = e.CallbackQuery.From.FirstName;
        await Program.Bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, "Великая битва!", replyMarkup: keyboard);
        battle.PreStart();
      }
    }


  }
}
