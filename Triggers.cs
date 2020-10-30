using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Requests;

namespace Bot_Fiona
{
  public class Triggers
  {
    public static Dictionary<string, string> triggers = new Dictionary<string, string>();
    private Telegram.Bot.Types.Message message;
    private TelegramBotClient Bot;
    private string[] commands = new string[] { "список", "Список", "/list", "Удалить", "Триггер", "Фиона", "фиона", "Девочка", "девочка", "/status", "/weather" };
    public Triggers()
    {
      LoadTrigers();
    }

    public Triggers(Telegram.Bot.Types.Message message, TelegramBotClient Bot)
    {
      this.message = message;
      this.Bot = Bot;
      LoadTrigers();
    }

    public async void FindTrigger()
    {
      if (message.Type == MessageType.Text && triggers.ContainsKey(message.Text))
      {
        string er = triggers[message.Text];
        

        switch (er)
        {
          case string trig when trig.Contains("DQA") && trig.Substring(0, 3) == "DQA":
            InputOnlineFile inputOnlineFile = new InputOnlineFile(er);
            await Bot.SendVideoNoteAsync(message.Chat.Id, inputOnlineFile, replyToMessageId: message.MessageId);
            break;
          case string trig when trig.Contains("AwA") && trig.Substring(0, 3) == "AwA":
            InputOnlineFile inputOnlineFile1 = new InputOnlineFile(er);
            await Bot.SendVoiceAsync(message.Chat.Id, inputOnlineFile1, replyToMessageId: message.MessageId);
            break;
          case string trig when trig.Contains("CAA") && trig.Substring(0, 3) == "CAA":
            await Bot.SendStickerAsync(message.Chat, triggers[message.Text]);
            break;
          case string trig when trig.Contains("AgA") && trig.Substring(0, 3) == "AgA":
            InputOnlineFile inputOnlineFile2 = new InputOnlineFile(er);
            await Bot.SendPhotoAsync(message.Chat.Id, inputOnlineFile2, replyToMessageId: message.MessageId);
            break;
          case string trig when trig.Contains("CgA") && trig.Substring(0, 3) == "CgA":
            InputOnlineFile inputOnlineFile4 = new InputOnlineFile(er);
            await Bot.SendDocumentAsync(message.Chat.Id, inputOnlineFile4, replyToMessageId: message.MessageId);
            break;
          case string trig when trig.Contains("vov") && trig.Substring(0, 3) == "vov":
            er = er.Replace("vov", "");
            // Удалить этот элемент кода
            try
            {
              await Bot.ForwardMessageAsync(message.Chat, message.Chat, Convert.ToInt32(er));
            }
            catch
            {
              await Bot.SendTextMessageAsync(message.Chat, "Пока что не могу(", replyToMessageId: message.MessageId);
            }
            break;
          case string trig when trig.Contains("BAA") && trig.Substring(0, 3) == "BAA":
            InputOnlineFile inputOnlineFile3 = new InputOnlineFile(er);
            await Bot.SendVideoAsync(message.Chat.Id, inputOnlineFile3, replyToMessageId: message.MessageId);
            break;
          default:
            await Bot.SendTextMessageAsync(message.Chat, er, replyToMessageId: message.MessageId);
            break;
        }
      }
    }

    public async void DeleteTrigger()
    {
      if (message.Text.Contains('*') && triggers.ContainsKey(message.Text.Split('*')[1]))
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

    public async void AddTrigger()
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
            string fileName = "";
            if (message.ReplyToMessage.Type == MessageType.Sticker)
            {
              var index = message.ReplyToMessage.Sticker.FileId;
              string key = message.Text.Split('*')[1];
              triggers.Add(key, index);
              SaveTriggers();
              await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");
              await Bot.SendStickerAsync(message.Chat, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
            }
            else if (message.ReplyToMessage.Type == MessageType.Document) fileName = message.ReplyToMessage.Document.FileId;
            else if (message.ReplyToMessage.Type == MessageType.Video) fileName = message.ReplyToMessage.Video.FileId;
            else if (message.ReplyToMessage.Type == MessageType.VideoNote) fileName = message.ReplyToMessage.VideoNote.FileId;
            else if (message.ReplyToMessage.Type == MessageType.Voice) fileName = message.ReplyToMessage.Voice.FileId;
            else if (message.ReplyToMessage.Type == MessageType.Photo)
            {
              string key = message.Text.Split('*')[1];
              var replyphoto = message.ReplyToMessage.Photo;
              var fileid = message.ReplyToMessage.Photo[replyphoto.Length - 1].FileId;
              triggers.Add(key, fileid);
              SaveTriggers();
              await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");
              await Bot.SendStickerAsync(message.Chat, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
            }
            if (fileName.Length > 0)
            {
              string key = message.Text.Split('*')[1];
              triggers.Add(key, fileName);
              SaveTriggers();
              await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");
              await Bot.SendStickerAsync(message.Chat, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");
            }
            else if (message.Type == MessageType.Text && message.ReplyToMessage.Text.Trim().Length > 0)
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

    private void SaveTriggers()
    {
      using (StreamWriter writer = File.CreateText("Triggers.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, triggers);
      }
    }

    public void LoadTrigers()
    {
      if (!File.Exists("Triggers.txt")) return;
      string json = File.ReadAllText("Triggers.txt");
      triggers = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json);
    }
  }
}
