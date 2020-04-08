using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Bot_Fiona;

namespace botfiona
{
  class Program
  {
    static public TelegramBotClient Bot;
    static public MessageEventArgs ames;
    static public Telegram.Bot.Types.ChatId chatid;
    

    static void Main(string[] args)
    {
      Bot = new TelegramBotClient("905671296:AAExzN80dNrlGv3KE_R6_6ta-rsi7Fpi7Y0");
      var me = Bot.GetMeAsync().Result;
      Bot.OnMessage += Get_Mes;
      Bot.StartReceiving();
      Console.ReadKey(); 
    }


    public static async void Get_Mes(object sender, MessageEventArgs e)
    {
     
      var message = e.Message;
      if (message.Type == MessageType.Text) ames = e;
      //commands
      if (message.Type == MessageType.Text && message.Text.Substring(0, 1) == "/")
      {
        CommandManager commandManager = new CommandManager(e);
        commandManager.CheckCommand(message.Text);

      }
      //triggers
      if(message.Type == MessageType.Text) message.Text = message.Text.ToLower();
      Triggers trig = new Triggers(message, Bot);
      trig.FindTrigger();
      //other features
      TextManager textManager = new TextManager(message, Bot);
      textManager.Selecter();

    }

  }

}

