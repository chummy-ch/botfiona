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

namespace botfiona
{
  class Program
  {
    static public TelegramBotClient Bot;
    static public MessageEventArgs ames;
    static public Telegram.Bot.Types.ChatId chatid;
    static DateTime time1 = new DateTime(2020, 1, 1, 13, 13, 13);
    static public Dictionary<string, string> triggers = new Dictionary<string, string>();
    static List<DataItem> tempdataitems = new List<DataItem>(triggers.Count);
    static string[] commands = new string[] { "список", "Список", "/list", "Удалить", "Триггер", "Фиона", "фиона", "Девочка", "девочка", "/status", "/weather" };
    static List<string> gamersId = new List<string>();
    static string[] quastions = new string[] { "кто", "у кого", "кого" };
    static string[] trues = new string[] { "Да!", "Конечно!", "Без сомнений!", "Лоол, а как же иначе!" };
    static string[] falses = new string[] { "Нет", "Конечно нет!", "Такого не можут быть!", "Фейк!" };
    static List<string> story = new List<string>();
    static public Dictionary<string, int> mes = new Dictionary<string, int>();
    static Battle battle;
    static RankManager rankManager;
    static InlineKeyboardMarkup keyboard;
    static public DateTime w8 = new DateTime();
    static public bool online = false;
    static public int index1 = 0;

    static void Main(string[] args)
    {
      Bot = new TelegramBotClient("905671296:AAExzN80dNrlGv3KE_R6_6ta-rsi7Fpi7Y0");
      var me = Bot.GetMeAsync().Result;
      LoadTrigers();
      LoadUname();
      LoadMes();
      LoadStory();
      Bot.OnMessage += Get_Mes;
      Bot.OnCallbackQuery += Bot_OnCallbackQuery;
      Bot.StartReceiving();
      rankManager = new RankManager();
      /*      personManager = new PersonManager();
      */
      foreach (string key in triggers.Keys)
      {
        tempdataitems.Add(new DataItem(key, triggers[key].ToString()));
      }
      Console.ReadKey();
    }

    private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
      var keyboard = e.CallbackQuery.Message.ReplyMarkup;
      var content = e.CallbackQuery.Data;
      var message = e.CallbackQuery;
      index1 = e.CallbackQuery.Message.MessageId;
      if (e.CallbackQuery.From.FirstName == keyboard.InlineKeyboard.ElementAt(0).ElementAt(0).Text)
      {
        await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Ты и так участвуешь, чучело...");
        return;
      }
      if (e.CallbackQuery.Data.Equals("Второй боец") && battle != null)
      {
        battle.SetSecondPlayer(e.CallbackQuery.From.Username);
        //secondp = e.CallbackQuery.From.Username;
        keyboard.InlineKeyboard.ElementAt(0).ElementAt(1).Text = e.CallbackQuery.From.FirstName;
        await Bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, "Великая битва!", replyMarkup: keyboard);
        battle.PreStart();
      }
    }

    public static async void Get_Mes(object sender, MessageEventArgs e)
    {
      var message = e.Message;
/*      if (message.Type == MessageType.Text) ames = e;
*/      if (message.Type == MessageType.Text && message.Text.Substring(0, 1) == "/")
      {
        CommandManager commandManager = new CommandManager();
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
        if(un.Length > 0)
        mes.Add(message.From.Username, 1);
      }
      SaveMes();

     

      if (message.Type == MessageType.Text)
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
        if (message.Text.Contains("триггер"))
        {
          if (message.ReplyToMessage != null)
          {
            if (message.ReplyToMessage.Type == MessageType.Text)
            {
              if (triggers.ContainsKey(message.Text.Split('*')[1].ToLower()))
              {
                await Bot.SendTextMessageAsync(message.Chat, "Такой триггер уже существует :3");
                await Bot.SendTextMessageAsync(message.Chat, triggers[message.Text.Split('*')[1]]);
              }
              else
              {
                if (message.ReplyToMessage.Type == MessageType.Sticker)
                {
                  var index = message.ReplyToMessage.Sticker.FileId;
                  string key = message.Text.Split('*')[1];
                  triggers.Add(key, index);
                  SaveTriggers();
                  await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");
                  await Bot.SendStickerAsync(message.Chat, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
                }
                else if (message.ReplyToMessage.Text.Trim().Length > 0)
                {
                  string key = (message.Text.Split('*')[1]);
                  key = key.Trim();
                  if (commands.Contains(message.ReplyToMessage.Text))
                  {
                    await Bot.SendTextMessageAsync(message.Chat, "Команды нельзя использовать для триггера");
                  }
                  else if (commands.Contains(key))
                  {
                    await Bot.SendTextMessageAsync(message.Chat, "Команды нельзя использовать для триггера");
                  }
                  else
                  {
                    triggers.Add(key, message.ReplyToMessage.Text);
                    SaveTriggers();
                    await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
                  }
                }
              }
            }
            else

            {
              if (triggers.ContainsKey(message.Text.Split('*')[1].ToLower()))
              {
                await Bot.SendTextMessageAsync(message.Chat, "Такой триггер уже существует :3");
                await Bot.SendTextMessageAsync(message.Chat, triggers[message.Text.Split('*')[1]]);
              }
              else
              {
                if (message.ReplyToMessage.Type == MessageType.Sticker)
                {
                  var index = message.ReplyToMessage.Sticker.FileId;
                  string key = message.Text.Split('*')[1];
                  triggers.Add(key, index);
                  SaveTriggers();
                  await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");
                  await Bot.SendStickerAsync(message.Chat, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
                }


                else if (message.ReplyToMessage.Type == MessageType.Voice || message.ReplyToMessage.Type == MessageType.VideoNote)
                {
                  string key = message.Text.Split('*')[1];
                  triggers.Add(key, "vov" + message.ReplyToMessage.MessageId.ToString());
                  SaveTriggers();
                  await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");

                }
                else if (message.ReplyToMessage.Type == MessageType.Photo)
                {
                  await Bot.SendTextMessageAsync(message.Chat.Id, "Not yet");
                }
                else if (message.ReplyToMessage.Text.Trim().Length > 0)
                {
                  if (commands.Contains(message.ReplyToMessage.Text))
                  {
                    await Bot.SendTextMessageAsync(message.Chat, "Команды нельзя использовать для триггера");

                  }
                  else
                  {
                    string key = message.Text.Split('*')[1];
                    if (commands.Contains(key))
                    {
                      await Bot.SendTextMessageAsync(message.Chat, "Команды нельзя использовать для триггера");
                    }
                    else
                    {
                      triggers.Add(key, message.ReplyToMessage.Text);
                      SaveTriggers();
                      await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");
                      await Bot.SendStickerAsync(message.Chat, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
                    }
                  }
                }
              }
            }
          }
          else
          {
            await Bot.SendTextMessageAsync(message.Chat, "Какой-то шрэк забыл прикрепить сообщение");
            await Bot.SendStickerAsync(message.Chat, "CAADAgADBwAD9OfCJS6YbVaPHbHaFgQ");
          }
        }
        if (message.Text.Contains("удалить"))
        {
          if (triggers.ContainsKey(message.Text.Split('*')[1]))
          {
            string key = message.Text.Split('*')[1];
            triggers.Remove(key);
            SaveTriggers();
            await Bot.SendTextMessageAsync(message.Chat, "Триггер удален!");
            await Bot.SendAnimationAsync(message.Chat, "CAADAgADBwADCsj5KxMVV9JlWEjqFgQ");
          }
          else
          {
            await Bot.SendTextMessageAsync(message.Chat, "Что-то пошло не так");
          }
        }

        if (message.Text.Length <= 5 && message.Text.Length >= 2 && message.Text.Substring(message.Text.Length - 2).Contains("да") || message.Text == "да") 
        {
          if (message == null) return;
          await Bot.SendTextMessageAsync(message.Chat, "Пизда", replyToMessageId: message.MessageId);
        }

      }   
      if (message.Type == MessageType.Text && triggers.ContainsKey(message.Text))
      {
        string er = "ошибка";
        triggers.TryGetValue(message.Text, out er);
        if (er.Length > 3)
        {
          if (er.Substring(0, 3) == "vov")
          {
            er = er.Replace("vov", "");
            await Bot.ForwardMessageAsync(message.Chat, message.Chat, Convert.ToInt32(er));
          }
          else if (er.Contains("CAA"))
          {
            await Bot.SendStickerAsync(message.Chat, triggers[message.Text]);
          }

          else
          {
            await Bot.SendTextMessageAsync(message.Chat, er, replyToMessageId: message.MessageId);
          }
        }
      }




      if (message.From.Username != null && message.Text == "/battle" && online == false && message.Chat.Title.Contains("arena"))
      {
        Console.WriteLine(1);
        chatid = message.Chat.Id;
        w8 = DateTime.Now;
        online = true;
        battle = new Battle(Bot, e);
        battle.SetFirstPlayer(message.From.Username);
        InlineKeyboardMarkup markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
              InlineKeyboardButton.WithCallbackData(message.From.FirstName),
              InlineKeyboardButton.WithCallbackData("Второй боец")
            }

          });

        await Bot.SendTextMessageAsync(message.Chat.Id, "Великая битва!", replyMarkup: markup);
      }

      if (message.Text == "фиона")
      {
        await Bot.SendTextMessageAsync(message.Chat, "Привет, я Фиона, чат-бот Болота 4 :3");
        await Bot.SendStickerAsync(message.Chat, "CAADAgADGQAD9OfCJRWWFn5c1beEFgQ");

        await Bot.SendTextMessageAsync(message.Chat, "Мои команды:\n /status - для вызова персонального статуса  \n /game_enter - войти игру в <кто> \n /list - для просмотра всех  триггеров \n Триггер *triggger_name* - для создания нового триггера \n /weather - показать прогноз погоды на сейчас \n Задать мне вопрос - Фиона,<вопрос>?");

      }

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

      if (message.Type == MessageType.Text && message.Text.Contains("девочка"))
      {
        await Bot.SendStickerAsync(message.Chat, "CAADAgADKwADqWElFEZQB5e23FxJFgQ");
        await Bot.SendStickerAsync(message.Chat, "CAADAgADyAEAArMeUCPRh9FVnGyWTRYE");
        await Bot.SendStickerAsync(message.Chat, "CAADAgADLAADqWElFNm7GHyxzP9LFgQ");
        await Bot.SendStickerAsync(message.Chat, "CAADAgAD0gEAArMeUCPGE2QnmWBiEhYE");

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





    static void SaveTriggers()
    {
      List<DataItem> tempdataitems = new List<DataItem>(triggers.Count);
      foreach (string key in triggers.Keys)
      {
        tempdataitems.Add(new DataItem(key, triggers[key].ToString()));
      }

      using (FileStream fs = new FileStream("triggers.xml", FileMode.OpenOrCreate))
      {
        XmlSerializer serializer = new XmlSerializer(typeof(List<DataItem>));
        // StringWriter sw = new StringWriter();
        // XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        // ns.Add("","");
        fs.SetLength(0);
        serializer.Serialize(fs, tempdataitems);
      }
    }

    static void LoadTrigers()
    {
      Dictionary<string, string> triggers = new Dictionary<string, string>();
      XmlSerializer xs = new XmlSerializer(typeof(List<DataItem>));

      using (FileStream fs = new FileStream("triggers.xml", FileMode.OpenOrCreate))
      {
        if (fs.Length > 0)
        {
          List<DataItem> templist = (List<DataItem>)xs.Deserialize(fs);
          foreach (DataItem di in templist)
          {
            triggers.Add(di.Key, di.Value);
          }
        }

      }

      Program.triggers = triggers;
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
      // XML -> JSON
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

