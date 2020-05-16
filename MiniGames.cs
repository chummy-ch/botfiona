using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Telegram.Bot;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Formatting = Newtonsoft.Json.Formatting;
using botfiona;
using System.Linq;
using Telegram.Bot.Types.Enums;

namespace Bot_Fiona
{
  public class MiniGames 
  {
    private List<string> gamersId = new List<string>();
    static string[] trues = new string[] { "Да!", "Конечно!", "Без сомнений!", "Лоол, а как же иначе!" };
    static string[] falses = new string[] { "Нет", "Конечно нет!", "Такого не можут быть!", "Фейк!" };
    private string[] questions = new string[] { "у кого-то", "кто-то", "кто", "у кого",  "кого" };
    private TelegramBotClient Bot;
    private Telegram.Bot.Types.Message message;

    public MiniGames(Telegram.Bot.Types.Message message)
    {
      this.Bot = Program.Bot;
      this.message = message;
      LoadUname();
    }
    public MiniGames()
    {
      LoadUname();
    }

    public async void GetPlayers()
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

    public void AddPlayer()
    {
      if (gamersId.Contains(message.From.Username))
      {
        Bot.SendTextMessageAsync(message.Chat.Id, "Тю, ты что, странный? Ты же уже играшеь!", replyToMessageId: message.MessageId);
      }
      else
      {
        gamersId.Add(message.From.Username);
        SaveUname();
        Bot.SendTextMessageAsync(message.Chat.Id, "Фига ты крут! Ты в игре!," + message.From.FirstName, replyToMessageId: message.MessageId);
      }
    }

    public async void Question()
    {
      if (!message.Text.Contains("?") || message == null) return;
      string word = "";
      int index = 5;
      bool y = false;
      if (message.Text.Length > 7) index = 9;
      for (int i = 0; i < questions.Length; i++)
      {
        if (message.Text.Length > index && message.Text.Contains(questions[i]))
        {
          index = questions[i].Length + message.Text.IndexOf(questions[i]);
          if (questions[i].Contains("у")) y = true;
          break;
        }
        if (i == questions.Length - 1) return;
      }
      if (message.Text.IndexOf(' ') > 3 && message.Text.IndexOf(' ') < index) word = message.Text.Substring(0, message.Text.IndexOf(' '));
      Random rnd = new Random();
      int rn = rnd.Next(0, gamersId.Count());
      string question = message.Text.Substring(index, message.Text.Length - index - 1);
      if (y == true) await Bot.SendTextMessageAsync(message.Chat.Id, "У" + " @" + gamersId[rn] + question, replyToMessageId: message.MessageId);
      else
      {
        if (word.Length > 1) question = word + question;
          await Bot.SendTextMessageAsync(message.Chat.Id, question + " @" + gamersId[rn], replyToMessageId: message.MessageId);
      }
    }

    public async void TrueOrFalse()
    {
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

    public void SaveUname()
    {
      using (StreamWriter writer = File.CreateText("Unames.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, gamersId);
      }
    }

    public void LoadUname()
    {
      if (!File.Exists("Unames.txt")) return;
      string json = File.ReadAllText("Unames.txt");
      gamersId = new JavaScriptSerializer().Deserialize<List<string>>(json);
    }
  }
}
