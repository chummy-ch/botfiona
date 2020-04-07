using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;

namespace Bot_Fiona
{
  public class TextManager
  {
    private List<string> messages = new List<string> { "триггер", "удалить" };
    private Telegram.Bot.Types.Message message;
    private TelegramBotClient Bot;

    public TextManager(Telegram.Bot.Types.Message message, TelegramBotClient Bot)
    {
      this.message = message;
      this.Bot = Bot;
    }

    public async void Selecter()
    {
      string mes = message.Text;
      switch (mes)
      {
        case string text when text.Contains("триггер"):
          Triggers trig = new Triggers(message, Bot);
          trig.AddTrigger();
          break;
        case string text when text.Contains("удалить"):
          Triggers trig1 = new Triggers(message, Bot);
          trig1.DeleteTrigger();
          break;
        case "фиона":
          await Bot.SendTextMessageAsync(message.Chat, "Привет, я Фиона, чат-бот Болота 4 :3");
          await Bot.SendStickerAsync(message.Chat, "CAADAgADGQAD9OfCJRWWFn5c1beEFgQ");
          await Bot.SendTextMessageAsync(message.Chat, "Мои команды:\n /status - для вызова персонального статуса  \n /game_enter - войти игру в <кто> \n /list - для просмотра всех  триггеров \n Триггер *triggger_name* - для создания нового триггера \n /weather - показать прогноз погоды на сейчас \n Задать мне вопрос - Фиона,<вопрос>?");
          break;
        case "девочка":
            await Bot.SendStickerAsync(message.Chat, "CAADAgADKwADqWElFEZQB5e23FxJFgQ");
            await Bot.SendStickerAsync(message.Chat, "CAADAgADyAEAArMeUCPRh9FVnGyWTRYE");
            await Bot.SendStickerAsync(message.Chat, "CAADAgADLAADqWElFNm7GHyxzP9LFgQ");
            await Bot.SendStickerAsync(message.Chat, "CAADAgAD0gEAArMeUCPGE2QnmWBiEhYE");
          break;
        case string text when text.Substring(message.Text.Length - 2).Contains("да"):
          if (message.Text.Length <= 5 && message.Text.Length >= 2 )
          {
            if (message == null) return;
            await Bot.SendTextMessageAsync(message.Chat, "Пизда", replyToMessageId: message.MessageId);
          }
          break;
        case string text when text.Length > 5 && text.Contains("?"):
          MiniGames min = new MiniGames(message);
          min.Quastion();
          break;
      }

    }


  }
}
