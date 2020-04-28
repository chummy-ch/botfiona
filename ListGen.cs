using Bot_Fiona;
using System.Globalization;
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
      Triggers trig = new Triggers();
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
          case string trig when trig.Length > 30:
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - <Длинное сообщение>\n";
            break;
          default:
            list += $"\n{Triggers.triggers.Keys.ToList()[i]} - {Triggers.triggers.Values.ToList()[i]}\n";
            break;
        }
      }
      await Bot.SendTextMessageAsync(message.Chat, list);
    }
  }
}
