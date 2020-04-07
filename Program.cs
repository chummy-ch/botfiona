﻿using System;
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
    static List<string> gamersId = new List<string>();
    static string[] quastions = new string[] { "кто", "у кого", "кого" };
    static string[] trues = new string[] { "Да!", "Конечно!", "Без сомнений!", "Лоол, а как же иначе!" };
    static string[] falses = new string[] { "Нет", "Конечно нет!", "Такого не можут быть!", "Фейк!" };
    static List<string> story = new List<string>();
    static public Dictionary<string, int> mes = new Dictionary<string, int>();
    static RankManager rankManager;

    static void Main(string[] args)
    {
      Bot = new TelegramBotClient("905671296:AAExzN80dNrlGv3KE_R6_6ta-rsi7Fpi7Y0");
      var me = Bot.GetMeAsync().Result;
      Triggers triggers = new Triggers();
      triggers.LoadTrigers();
      LoadUname();
      LoadMes();
      LoadStory();
      Bot.OnMessage += Get_Mes;
      Bot.StartReceiving();
      rankManager = new RankManager();
      Console.ReadKey();
    }


    public static async void Get_Mes(object sender, MessageEventArgs e)
    {
     
      var message = e.Message;
      if (message.Type == MessageType.Text) ames = e;
      if (message.Type == MessageType.Text && message.Text.Substring(0, 1) == "/")
      {
        CommandManager commandManager = new CommandManager(e);
        commandManager.CheckCommand(message.Text);

      }
      if (message.Text == "rerre")          // доделать список типа Person
      {
        Battle.LoadWins();
        Battle battle = new Battle();
        string name = message.From.Username;
        Person p = new Person();
        p.MakePerson(name, mes[name], Battle.pwins[name]);
      }

      try
      {
        string un = message.From.Username.Trim();
        if (un.Length > 0 && mes.ContainsKey(un))
        {
          mes[message.From.Username] += 1;
          if (rankManager.CountExists(mes[message.From.Username]))
          {
            await Bot.SendTextMessageAsync(message.Chat.Id, string.Format("Поздравляю!🎉 \nВы достигли ранга: {0}",
              rankManager.GetRank(mes[message.From.Username])), replyToMessageId: message.MessageId);
          }

        }
        else
        {
          if (un.Length > 0)
            mes.Add(message.From.Username, 1);
        }
        SaveMes();
      }
      catch
      {
        Console.WriteLine("No username");
      }

      Triggers trig = new Triggers(message, Bot);
      trig.FindTrigger();

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

      if (message.Text == "/players")
      {
        string spis = "Игроки:";
        int i = 1;
        foreach (string s in gamersId)
        {
          spis += "\n" + i + ". @" + s;
          i += 1;
        }
        await Bot.SendTextMessageAsync(message.Chat.Id, spis);
      }

      if (message.Type == MessageType.Text && message.Text.Contains("/game"))
      {
        if (gamersId.Contains(message.From.Username))
        {
          await Bot.SendTextMessageAsync(message.Chat.Id, "Тю, ты что, странный? Ты же уже играшеь!", replyToMessageId: message.MessageId);
        }
        else
        {
          gamersId.Add(message.From.Username);
          SaveUname();
          await Bot.SendTextMessageAsync(message.Chat.Id, "Фига ты крут! Ты в игре!," + message.From.FirstName, replyToMessageId: message.MessageId);
        }
      }

      if (message.Type == MessageType.Text && message.Text.Length > 5)
      {
        if (message.Type == MessageType.Text && message.Text.Substring(0, 5).Contains(quastions[0]) || message.Text.Substring(0, 5).Contains(quastions[1]) || message.Text.Substring(0, 5).Contains(quastions[2]))
        {
          if (message.Text.Contains("?") && message != null)
          {
            Random rnd = new Random();
            int rn = rnd.Next(0, gamersId.Count());
            if (message.Text.Contains(quastions[0]))
            {

              if (message.Text.Length > 5)
              {
                string mat = message.Text.Substring(4, message.Text.Length - 5);
                await Bot.SendTextMessageAsync(message.Chat.Id, mat + " @" + gamersId[rn], replyToMessageId: message.MessageId);
              }

            }
            else if (message.Text.Contains(quastions[1]))
            {
              if (message.Text.Length > 8)
              {
                string mat = message.Text.Substring(7, message.Text.Length - 8);
                await Bot.SendTextMessageAsync(message.Chat.Id, mat + " у" + " @" + gamersId[rn], replyToMessageId: message.MessageId);
              }
            }
            else if (message.Text.Contains(quastions[2]))
            {
              if (message.Text.Length > 6)
              {
                string mat = message.Text.Substring(5, message.Text.Length - 6);
                await Bot.SendTextMessageAsync(message.Chat.Id, mat + " @" + gamersId[rn], replyToMessageId: message.MessageId);
              }
            }
          }
        }

      }

     

      if (message.Type == MessageType.Text && message.Text.Contains("фиона,") && message.Text.Contains("?") && message.Text.Length > 7)
      {
        string quash = message.Text.Substring(7, message.Text.Length - 8);
        quash = quash.Replace(" ", "");
        Random rnd = new Random();
        int rn = rnd.Next(0, 3);
        if (quash.Length > 0)
        {
          if (quash.Length % 2 == 0) await Bot.SendTextMessageAsync(message.Chat.Id, trues[rn], replyToMessageId: message.MessageId);
          else await Bot.SendTextMessageAsync(message.Chat.Id, falses[rn], replyToMessageId: message.MessageId);
        }
        else await Bot.SendStickerAsync(message.Chat.Id, "CAADAgADBwAD9OfCJS6YbVaPHbHaFgQ", replyToMessageId: message.MessageId);

      }

    }





   

    static void SaveUname()
    {
      using (FileStream fs = new FileStream("Unames.xml", FileMode.OpenOrCreate))
      {
        XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
        fs.SetLength(0);
        serializer.Serialize(fs, gamersId);
      }
    }

    static void LoadUname()
    {
      XmlSerializer xs = new XmlSerializer(typeof(List<string>));
      using (FileStream fs = new FileStream("Unames.xml", FileMode.OpenOrCreate))
      {
        List<string> templist = (List<string>)xs.Deserialize(fs);
        foreach (string di in templist)
        {
          gamersId.Add(di);
        }
      }
    }

    static void SaveMes()
    {
      using (StreamWriter writer = File.CreateText("mes.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, mes);
      }
    }

    static void LoadMes()
    {
      if (!File.Exists("mes.txt")) return;
      string json = File.ReadAllText("mes.txt");
      mes = new JavaScriptSerializer().Deserialize<Dictionary<string, int>>(json);
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

