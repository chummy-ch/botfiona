using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Bot_Fiona;
using System.Runtime.InteropServices;

namespace botfiona
{
  class Program
  {
    static public TelegramBotClient Bot;
    static public MessageEventArgs ames;

    static void Main(string[] args)
    {

      Bot = new TelegramBotClient(APIData.key);
      Bot.OnMessage += Get_Mes;
      Bot.StartReceiving();
     
      Console.ReadKey();
    }


    public static void Get_Mes(object sender, MessageEventArgs e)
    {
      var message = e.Message;
      if (message.Type == MessageType.Text) ames = e;

      TextManager text = new TextManager(message, Bot);
      text.Counter();
      if (DateTime.Now.Subtract(message.Date).TotalMinutes >= 130 && message.Type == MessageType.Text && !message.Text.Contains("удалить") && !message.Text.Contains("триггер")) return;
      //commands
      if (message.Type == MessageType.Text && message.Text.Substring(0, 1) == "/")
      {
        CommandManager commandManager = new CommandManager(e);
        commandManager.CheckCommand(message.Text);

      }

      //triggers
      if (message.Type == MessageType.Text) message.Text = message.Text.ToLower();
      Triggers trig = new Triggers(message, Bot);
      trig.FindTrigger();

      //other features
      text.Selecter();
    }
  }
}

