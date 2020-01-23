using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace botfiona
{
  class ListGen
  {
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;

    public ListGen()
    {

    }

    public async void GetList()
    {
      var message = m.Message;
      string list = "Команды:";
      for (int i = 0; i < Program.triggers.Count; i++)
      {

        if (Program.triggers.Values.ToList()[i].Contains("CAA"))
        {
          list += "\n";
          list += $"{Program.triggers.Keys.ToList()[i]} - <стикер>";
          list += "\n";
        }
        else if (Program.triggers.Values.ToList()[i].Length > 40)
        {
          list += "\n";
          list += $"{Program.triggers.Keys.ToList()[i]} - <Длинное значение>";
          list += "\n";
        }
        else if (Program.triggers.Values.ToList()[i].Contains("www.") || Program.triggers.Values.ToList()[i].Contains("@gmail.") || Program.triggers.Values.ToList()[i].Contains("@nure.") || Program.triggers.Values.ToList()[i].Contains("tss."))
        {
          list += "\n";
          list += $"{Program.triggers.Keys.ToList()[i]} - <url>";
          list += "\n";
        }
        else if (Program.triggers.Values.ToList()[i].Length > 3 && Program.triggers.Values.ToList()[i].Substring(0, 3).Contains("vov"))
        {
          list += "\n";
          list += $"{Program.triggers.Keys.ToList()[i]} - <media>";
          list += "\n";
        }
        else
        {
          list += "\n";
          list += $"{Program.triggers.Keys.ToList()[i]} - {Program.triggers.Values.ToList()[i]}";
          list += "\n";
        }
      }
      await Bot.SendTextMessageAsync(message.Chat, list);
    }
  }
}
