using Bot_Fiona;
using System.Linq;
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
      for (int i = 0; i < Triggers.triggers.Count; i++)
      {

        if (Triggers.triggers.Values.ToList()[i].Contains("CAA"))
        {
          list += "\n";
          list += $"{Triggers.triggers.Keys.ToList()[i]} - <стикер>";
          list += "\n";
        }
        else if (Triggers.triggers.Values.ToList()[i].Length > 40)
        {
          list += "\n";
          list += $"{Triggers.triggers.Keys.ToList()[i]} - <Длинное значение>";
          list += "\n";
        }
        else if (Triggers.triggers.Values.ToList()[i].Contains("www.") || Triggers.triggers.Values.ToList()[i].Contains("@gmail.") || Triggers.triggers.Values.ToList()[i].Contains("@nure.") || Triggers.triggers.Values.ToList()[i].Contains("tss."))
        {
          list += "\n";
          list += $"{Triggers.triggers.Keys.ToList()[i]} - <url>";
          list += "\n";
        }
        else if (Triggers.triggers.Values.ToList()[i].Length > 3 && Triggers.triggers.Values.ToList()[i].Substring(0, 3).Contains("vov"))
        {
          list += "\n";
          list += $"{Triggers.triggers.Keys.ToList()[i]} - <media>";
          list += "\n";
        }
        else
        {
          list += "\n";
          list += $"{Triggers.triggers.Keys.ToList()[i]} - {Triggers.triggers.Values.ToList()[i]}";
          list += "\n";
        }
      }
      await Bot.SendTextMessageAsync(message.Chat, list);
    }
  }
}
