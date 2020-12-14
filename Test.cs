using botfiona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Bot_Fiona
{
  class Test
  {
    private TelegramBotClient bot = Program.Bot;
    private MessageEventArgs m = Program.ames;

    public Test()
    {

    }

    public async void testing()
    {
      var message = m.Message;
      String text = "#Hello *World*";
      Telegram.Bot.Types.Message mes = await bot.SendTextMessageAsync(message.Chat.Id, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
    }

  }
}
