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
  class TextManager
  {
    private List<string> messages = new List<string> { "триггер", "удалить" };
    private Telegram.Bot.Types.Message message;
    private TelegramBotClient Bot;

    TextManager(Telegram.Bot.Types.Message message, TelegramBotClient Bot)
    {
      this.message = message;
      this.Bot = Bot;
    }

    public void Selecter()
    {
      
    }

    
  }
}
