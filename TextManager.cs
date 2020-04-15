using System;
using System.Collections.Generic;
using Telegram.Bot;
using botfiona;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Bot_Fiona
{
  public class TextManager
  {
    private List<string> messages = new List<string> { "триггер", "удалить" };
    private Telegram.Bot.Types.Message message;
    public static Dictionary<string, int> mesCount = new Dictionary<string, int>();
    private TelegramBotClient Bot;

    public TextManager(Telegram.Bot.Types.Message message, TelegramBotClient Bot)
    {
      this.message = message;
      this.Bot = Bot;
      LoadMes();
    }

    public TextManager() 
    {
      LoadMes();
    }

    public async void Selecter()
    {
      string mes = message.Text;
      RankManager rankManager = new RankManager();
      try
      {
        long boloto = long.Parse("-1001100135301");
        if (message.Chat.Id == boloto) 
        {
          string un = message.From.Username.Trim();
          if (un.Length > 0 && mesCount.ContainsKey(un))
          {
            mesCount[message.From.Username] += 1;
            if (rankManager.CountExists(mesCount[message.From.Username]))
            {
              await Bot.SendTextMessageAsync(message.Chat.Id, string.Format("Поздравляю!🎉 \nВы достигли ранга: {0}",
                rankManager.GetRank(mesCount[message.From.Username])), replyToMessageId: message.MessageId);
            }

          }
          else
          {
            if (un.Length > 0)
              mesCount.Add(message.From.Username, 1);
          }
          SaveMes();
        }
      }
        
      catch
      {
        Console.WriteLine("No username");
      }
      switch (mes)
      {
        case string text when text.Contains("триггер") && text.Contains("*"):
          
          Triggers trig = new Triggers(message, Bot);
          trig.AddTrigger();
          break;
        case string text when text.Contains("удалить"):
          Triggers trig1 = new Triggers(message, Bot);
          trig1.DeleteTrigger();
          break;
        case "фиона":
          await Bot.SendTextMessageAsync(message.Chat, "Привет, я Фиона, чат-бот Болота 4 :3");
          await Bot.SendStickerAsync(message.Chat, "CAADAgADGQAD9OfCJRWWFn5c1beEFgQ");
          await Bot.SendTextMessageAsync(message.Chat, "Мои команды:\n /status - для вызова персонального статуса  \n /game_enter - войти игру в <кто> \n /list - для просмотра всех  триггеров \n Триггер *triggger_name* - для создания нового триггера \n /weather - показать прогноз погоды на сейчас \n Задать мне вопрос - Фиона,<вопрос>?");
          break;
        case "девочка":
            await Bot.SendStickerAsync(message.Chat, "CAADAgADKwADqWElFEZQB5e23FxJFgQ");
            await Bot.SendStickerAsync(message.Chat, "CAADAgADyAEAArMeUCPRh9FVnGyWTRYE");
            await Bot.SendStickerAsync(message.Chat, "CAADAgADLAADqWElFNm7GHyxzP9LFgQ");
            await Bot.SendStickerAsync(message.Chat, "CAADAgAD0gEAArMeUCPGE2QnmWBiEhYE");
          break;
        case string text when text.Length > 2 && text.Substring(message.Text.Length - 2).Contains("да"):
          if (message.Text.Length <= 5 && message.Text.Length >= 2 )
          {
            if (message == null) return;
            await Bot.SendTextMessageAsync(message.Chat, "Пизда", replyToMessageId: message.MessageId);
          }
          break;
        case string text when text.Length > 8 && text.Substring(0, 6) == "фиона," && text.Contains("?"):
          MiniGames trueorfalse = new MiniGames(message);
          trueorfalse.TrueOrFalse();
          break;
        case string text when text.Length > 5 && text.Contains("?"):
          MiniGames min = new MiniGames(message);
          min.Quastion();
          break;
        
          
      }

    }
    public void SaveMes()
    {
      using (StreamWriter writer = File.CreateText("mes.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, mesCount);
      }
    }

    public void LoadMes()
    {
      if (!File.Exists("mes.txt")) return;
      string json = File.ReadAllText("mes.txt");
      mesCount = new JavaScriptSerializer().Deserialize<Dictionary<string, int>>(json);
    }


  }
}
