using Bot_Fiona;
using System;
using System.Globalization;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace botfiona
{
  class ListGen
  {
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;
    private int page = 0;
    private long freshBoard;

    public ListGen()
    {
      Triggers trig = new Triggers();
      Bot.OnCallbackQuery += bot_OnCallbackQuery;
    }

    public async void GetList()
    {
      var message = m.Message;
      string list = "Команды:";
      for (int i = 0; i < Triggers.triggers.Count; i++)
      {
        switch (Triggers.triggers.Values.ToList()[i])
        {
          case string trig when trig.Contains("CAA"):
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <стикер>\n";
            break;
          case string trig when trig.Contains(".ua") || trig.Contains("www.") || trig.Contains(".com") || trig.Contains("tss."):
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <url>\n";
            break;
          case string trig when trig.Contains("vov") || trig.Contains("AwA") || trig.Contains("DQA") || trig.Contains("AgA"):
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <media>\n";
            break;
          case string trig when trig.Length > 40:
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <Длинное сообщение>\n";
            break;
          default:
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - {Triggers.triggers.Values.ToList()[i]}\n";
            break;
        }
      }
      await Bot.SendTextMessageAsync(message.Chat, list);
    }

    public string GetInteractiveList()
    {
      int index = page * 10;
      if(index > Triggers.triggers.Count)
      {
        index = --page * 10;
      }
      string list = "Команды:\n";
      int len = index + 10;
      if (len > Triggers.triggers.Count) len = Triggers.triggers.Count;
      for(int i = index; i < len; i++)
      {
        switch (Triggers.triggers.Values.ToList()[i])
        {
          case string trig when trig.Contains("CAA"):
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <стикер>\n";
            break;
          case string trig when trig.Contains(".ua") || trig.Contains("www.") || trig.Contains(".com") || trig.Contains("tss."):
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <url>\n";
            break;
          case string trig when trig.Contains("vov") || trig.Contains("AwA") || trig.Contains("DQA") || trig.Contains("AgA"):
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <media>\n";
            break;
          case string trig when trig.Length > 40:
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <Длинное сообщение>\n";
            break;
          default:
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - {Triggers.triggers.Values.ToList()[i]}\n";
            break;
        }
      }
      return list;
    }

    public async void CreateList()
    {
      var message = m.Message;
      string list = GetInteractiveList();
      InlineKeyboardMarkup listButtons = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithCallbackData("<<"),
          InlineKeyboardButton.WithCallbackData("<"),
          InlineKeyboardButton.WithCallbackData(">"),
          InlineKeyboardButton.WithCallbackData(">>"),
        }
      });
      var result = Bot.SendTextMessageAsync(message.Chat.Id, list, replyMarkup: listButtons).Result;
      freshBoard = result.MessageId;
    }

    private void bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      if (e.CallbackQuery.Message.MessageId != freshBoard) return;
      if (e.CallbackQuery.Data == "<") --page;
      else if (e.CallbackQuery.Data == "<<") page -= 2;
      else if (e.CallbackQuery.Data == ">") ++page;
      else if (e.CallbackQuery.Data == ">>") page += 2;
      if (page < 0) page = 0;
      else if (page > Triggers.triggers.Count / 10) page = Triggers.triggers.Count / 10;
      Bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, GetInteractiveList(), replyMarkup: e.CallbackQuery.Message.ReplyMarkup);
      Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, e.CallbackQuery.Data);
    }
  }
}
