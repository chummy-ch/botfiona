using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

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
    }

    public async void DeleteTrigger()
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
