using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Telegram.Bot;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Formatting = Newtonsoft.Json.Formatting;
using Telegram.Bot.Args;
using botfiona;
using System.Linq;

namespace Bot_Fiona
{
  public class MiniGames
  {
    private List<string> gamersId = new List<string>();
    private string[] quastions = new string[] { "у кого-то", "кто-то", "кто", "у кого",  "кого" };
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

    public async void Quastion()
    {
      if (!message.Text.Contains("?") || message == null) return;
      int index = 5;
      bool y = false;
      if (message.Text.Length > 7) index = 9;
      for (int i = 0; i < quastions.Length; i++)
      {
        if (message.Text.Substring(0, index).Contains(quastions[i]))
        {
          index = quastions[i].Length;
          if (quastions[i].Contains("у")) y = true;
          break;
        }
        if (i == quastions.Length - 1) return;
      }

      Random rnd = new Random();
      int rn = rnd.Next(0, gamersId.Count());
      string mat = message.Text.Substring(index, message.Text.Length - index - 1);
      if(y == false) await Bot.SendTextMessageAsync(message.Chat.Id, mat + " @" + gamersId[rn], replyToMessageId: message.MessageId);
      else await Bot.SendTextMessageAsync(message.Chat.Id, "У" + " @" + gamersId[rn], replyToMessageId: message.MessageId);

      /* if (message.Text.Contains(quastions[0]))
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
       }*/

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
