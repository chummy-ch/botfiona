using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using HtmlAgilityPack;


namespace botfiona
{
  class Weather
  {
    private DateTime time1 = new DateTime(2020, 1, 1, 13, 13, 13);
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;
    public Weather()
    {
    }

    public async void GetWeather()
    {
      var message = m.Message;
      if (DateTime.Now.Subtract(time1).TotalSeconds > 180)
      {
        time1 = DateTime.Now;
        Random rnd = new Random();
        int rn = rnd.Next(1, 4);
        switch (rn)
        {
          case 1:
            await Bot.SendTextMessageAsync(message.Chat, "Такс, посмотрим, что у нас тут за погода на Болоте...");
            await Bot.SendTextMessageAsync(message.Chat, "Звоню погодной фее...  🧚‍♂️");
            break;
          case 2:
            await Bot.SendTextMessageAsync(message.Chat, "На градусник посмотреть слабо? 🌡");
            await Bot.SendTextMessageAsync(message.Chat, "Ну ладно, щас зайду на Gismeteo...");
            break;
          case 3:
            await Bot.SendTextMessageAsync(message.Chat, "Лучше бы вы делали ВМ :3");
            await Bot.SendTextMessageAsync(message.Chat, "Кости ломит....");

            break;
        }
        string url = "https://www.gismeteo.ua/weather-kharkiv-5053/";
        var web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        var t = doc.DocumentNode.SelectSingleNode("/html/body/section/div[2]/div/div[1]/div/div[2]/div[1]/div[1]/a[1]/div/div[1]/div[3]/div[1]/span[1]/span");
        string temp = t.InnerText;
        if (temp.Contains(","))
        {
          int index = temp.IndexOf(",");
          temp = temp.Substring(0, index);
        }

        temp = temp.Trim();


        if (temp.Contains("&minus;")) temp.Replace("&minus;", "-");
        var c = doc.DocumentNode.SelectSingleNode("/html/body/section/div[2]/div/div[1]/div/div[2]/div[1]/div[1]/a[1]");
        string cond = c.Attributes["data-text"].Value;


        if (temp.Contains("&minus;"))
        {
          temp = temp.Replace("&minus;", "-");
          temp = temp.Trim();
          await Bot.SendTextMessageAsync(message.Chat, $"На улице сейчас.... \n ❄️{temp}❄️");
        }
        else if (Convert.ToInt32(temp) > -1 && Convert.ToInt32(temp) < 10)
        {
          await Bot.SendTextMessageAsync(message.Chat, $"На улице сейчас.... \n ✨{temp}✨");
        }
        else
        {
          await Bot.SendTextMessageAsync(message.Chat, $"На улице сечас....  \n ☀️{temp}☀️");
        }

        if (cond == "Ясно")
        {
          await Bot.SendStickerAsync(message.Chat, "CAADAgADOQIAAs7Y6AtiQa4j611amhYE");
          await Bot.SendTextMessageAsync(message.Chat, $"{cond}");
        }
        else if (cond == "Переменная облачность")
        {
          await Bot.SendStickerAsync(message.Chat, "CAADAgADRwQAAs7Y6AtUgM8Qt1L1BBYE");
          await Bot.SendTextMessageAsync(message.Chat, $"{cond}");
        }
        else if (cond.Contains("Пасмурно") && cond.Contains("дождь"))
        {
          await Bot.SendStickerAsync(message.Chat, "CAADAgAD8AEAAs7Y6Av_YmkSfuc8BhYE");
          await Bot.SendTextMessageAsync(message.Chat, $"{cond}");
        }
        else if (cond.Contains("Пасмурно") || cond.Contains("Облачно"))
        {
          await Bot.SendStickerAsync(message.Chat, "CAADAgADDwIAAtzyqwflTv80MV32fhYE");
          await Bot.SendTextMessageAsync(message.Chat, $"{cond}");
        }
        else await Bot.SendTextMessageAsync(message.Chat, $"{cond}");
      }
      else await Bot.SendTextMessageAsync(message.Chat.Id, "Погоду можно запрашивать раз в 3 минуты", replyToMessageId: message.MessageId);


    }
  }
}
