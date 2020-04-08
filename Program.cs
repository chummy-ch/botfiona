using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Threading;
using Bot_Fiona;

namespace botfiona
{
  class Program
  {
    static public TelegramBotClient Bot;
    static public MessageEventArgs ames;
    static public Telegram.Bot.Types.ChatId chatid;
    static List<string> story = new List<string>();
    

    static void Main(string[] args)
    {
      Bot = new TelegramBotClient("905671296:AAExzN80dNrlGv3KE_R6_6ta-rsi7Fpi7Y0");
      var me = Bot.GetMeAsync().Result;
      Triggers triggers = new Triggers();
      triggers.LoadTrigers();
      LoadStory();
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


/*      if (message.Type == MessageType.Text)
      {
        message.Text = message.Text.ToLower();
        if (message.Text.Length > 15)
        {
          for (int i = 0; i < message.Text.Split(' ').Length; i++)
          {
            story.Add(message.Text.Split(' ')[i]);
          }
        }
        else story.Add(message.Text);
        SaveStory();
        if (message.Text == "фиона, история")
        {
          Random rdn = new Random();
          int nr = rdn.Next(3, story.Count);
          Random rnd = new Random();
          string storys = "И так: ";
          for (int i = 1; i <= nr; i++)
          {
            if (storys.Length > 3000) break;
            int rn = rnd.Next(0, story.Count - 1);
            storys += story[rn] + " ";
            story.RemoveAt(rn);
            i++;
          }
          await Bot.SendTextMessageAsync(message.Chat.Id, storys);
          storys += "";
        }
      }
*/

     
      

    }





   

   

   

    static void SaveStory()
    {
      using (StreamWriter writer = File.CreateText("Story.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, story);
      }
    }

    static void LoadStory()
    {
      if (!File.Exists("Story.txt")) return;
      string json = File.ReadAllText("Story.txt");
      /*story = new JavaScriptSerializer().Deserialize<List<string>>(json);*/
    }

  }

}

