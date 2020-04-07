using botfiona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot_Fiona
{
  public class BattleManager
  {
    private MessageEventArgs e;
    static public bool online = false;
    public  TelegramBotClient Bot;
    static Battle battle; 
    public static int index1 = 0;


    public BattleManager(MessageEventArgs e)
    {
      this.e = e;
      this.Bot = Program.Bot;
      battle = new Battle(Bot, e);
      Bot.OnCallbackQuery += Bot_OnCallbackQuery;
    }


    public async void PreBattle()
    {
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

        await Bot.SendTextMessageAsync(message.Chat.Id, "Великая битва!", replyMarkup: markup);
      }
      
    }

    private  async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      var keyboard = e.CallbackQuery.Message.ReplyMarkup;
      var content = e.CallbackQuery.Data;
      var message = e.CallbackQuery;
      index1 = e.CallbackQuery.Message.MessageId;
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
