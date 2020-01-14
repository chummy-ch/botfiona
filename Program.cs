using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using HtmlAgilityPack;
using Telegram.Bot.Types.ReplyMarkups;
namespace botfiona
{
    class Program
    {
        static TelegramBotClient Bot;
        static DateTime time1 = new DateTime(2020, 1, 1, 13, 13, 13);
        static Dictionary<string, string> triggers = new Dictionary<string, string>();
        static List<DataItem> tempdataitems = new List<DataItem>(triggers.Count);
        static string[] commands = new string[] { "список", "Список", "/list", "Удалить", "Триггер", "Фиона", "фиона", "Девочка", "девочка", "погода", "Погода" };
        static List<string> gamersId = new List<string>();
        static string[] quastions = new string[] { "кто", "у кого", "кого" };
        static string[] trues = new string[] { "Да!", "Конечно!", "Без сомнений!", "Лоол, а как же иначе!" };
        static string[] falses = new string[] { "Нет", "Конечно нет!", "Такого не можут быть!", "Фейк!" };
        static List<string> story = new List<string>();
        static List<string> people = new List<string>();
        static List<int> count = new List<int>();
        static string[] ranks = new string[] { "Глеб", "Конч", "Мамонтов", "Харьковчанен", "Хнурешник", "Языковая шаболда", "Болотный барт" };
        static string[] rankspic = new string[] { "https://i1.rgstatic.net/ii/profile.image/797915803029504-1567249360132_Q512/Glib_Tereshchenko.jpg" };
        static List<int> ranksc = new List<int> { 150, 800, 2000, 5000, 10000, 25000, 88888, 100000 };

        static InlineKeyboardMarkup keyboard;


        static void Main(string[] args)
        {
            Bot = new TelegramBotClient("905671296:AAFcDT4qymtle-QyUne4agx14q_97mIQMXI");
            var me = Bot.GetMeAsync().Result;
            LoadTrigers();
            LoadUname();
            LoadPeople();
            LoadCount();
            Bot.OnMessage += Get_Mes;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();

            foreach (string key in triggers.Keys)
            {
                tempdataitems.Add(new DataItem(key, triggers[key].ToString()));
            }

            Console.ReadKey();
        }

        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery;
            string name = e.CallbackQuery.From.FirstName;
            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "ну и че");

            InlineKeyboardButton btn = null;
            foreach (var row in keyboard.InlineKeyboard)
            {
                foreach (var button in row)
                {
                    if (button.CallbackData == e.CallbackQuery.Data) btn = button;
                }
            }

            btn.Text = "A";
        }

        private static async void Get_Mes(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            /*            Console.WriteLine(message.Chat.Id);
            */
            if (message.Type == MessageType.Sticker) Console.WriteLine(message.Sticker.FileId);
            if (message.Chat.Id != -1001100135301 && message.Chat.Id != 361119003)
            {
                await Bot.SendTextMessageAsync(361119003, "@" + message.From.Username);
                await Bot.ForwardMessageAsync(361119003, message.Chat.Id, message.MessageId);
                await Bot.SendTextMessageAsync(message.Chat.Id, "Попросите разрешения на использование  у @chummych 🤴");
            }
            if (message.Chat.Id == -1001100135301 || message.Chat.Id == 361119003)
            {
                if (message.Chat.Id == -1001100135301 && message.From.Username != null || message.Chat.Id == 361119003 )
                {
                    if (message.Type == MessageType.Text)
                    {
                        if(message.Text.Contains("changecount"))
                        {
                            string r = message.Text.Substring(12, message.Text.Length - 12);
                            count[0] = Convert.ToInt32(r);
                        }
                        if (people.Contains(message.From.Username))
                        {
                            int index = people.IndexOf(message.From.Username);
                            if (count.Count > people.IndexOf(message.From.Username))
                            {
                                count[index] += 1;
                                SaveCount();
                            }

                            else
                            {
                                count.Add(1);
                                SaveCount();
                            }
                        }
                        else
                        {
                            people.Add(message.From.Username);
                            int index = people.IndexOf(message.From.Username);
                            count[index] += 1;
                            SavePeople();
                            SaveCount();
                        }
                        if (ranksc.Contains(count[people.IndexOf(message.From.Username)]))
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, $"Поздравляю!🎉 \n Вы достигли ранга: {ranks[ranksc.IndexOf(count[people.IndexOf(message.From.Username)]) + 1]}", replyToMessageId: message.MessageId);
                        }
                    }
                    else
                    {
                        if (people.Contains(message.From.Username))
                        {
                            int index = people.IndexOf(message.From.Username);
                            if (count.Count > people.IndexOf(message.From.Username))
                            {
                                count[index] += 1;
                                SaveCount();
                            }

                            else
                            {
                                count.Add(1);
                                SaveCount();
                            }
                        }
                        else
                        {
                            people.Add(message.From.Username);
                            int index = people.IndexOf(message.From.Username);
                            count[index] += 1;
                            SavePeople();
                            SaveCount();
                        }
                        if (ranksc.Contains(count[people.IndexOf(message.From.Username)]))
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, $"Поздравляю!🎉 Вы достигли ранга: {ranks[ranksc.IndexOf(count[people.IndexOf(message.From.Username)])]}");
                        }
                    }
                }

                if (message.Type == MessageType.Text)
                {
                    for (int i = 0; i < message.Text.Split(' ').Length; i++)
                    {
                        story.Add(message.Text.Split(' ')[i]);
                    }
                    story.Add(message.Text);
                    message.Text = message.Text.ToLower();

                    if (message.Text == "фиона, история")
                    {
                        Random rdn = new Random();
                        int nr = rdn.Next(3, story.Count);
                        Random rnd = new Random();
                        string storys = "И так: ";
                        for (int i = 1; i <= nr; i++)
                        {
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
                                        Console.WriteLine(index);
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


                    if (message.Text == "/list" || message.Text == "список")
                    {
                        string list = "Команды:";

                        for (int i = 0; i < triggers.Count; i++)
                        {

                            if (triggers.Values.ToList()[i].Contains("CAA"))
                            {
                                list += "\n";
                                list += $"{triggers.Keys.ToList()[i]} - <стикер>";
                                list += "\n";
                            }
                            else if (triggers.Values.ToList()[i].Length > 40)
                            {
                                list += "\n";
                                list += $"{triggers.Keys.ToList()[i]} - <Длинное значение>";
                                list += "\n";
                            }
                            else if (triggers.Values.ToList()[i].Contains("www.") || triggers.Values.ToList()[i].Contains("@gmail.") || triggers.Values.ToList()[i].Contains("@nure.") || triggers.Values.ToList()[i].Contains("tss."))
                            {
                                list += "\n";
                                list += $"{triggers.Keys.ToList()[i]} - <url>";
                                list += "\n";
                            }
                            else if (triggers.Values.ToList()[i].Length > 3 && triggers.Values.ToList()[i].Substring(0,3).Contains("vov"))
                            {
                                list += "\n";
                                list += $"{triggers.Keys.ToList()[i]} - <media>";
                                list += "\n";
                            }
                            else
                            {
                                list += "\n";
                                list += $"{triggers.Keys.ToList()[i]} - {triggers.Values.ToList()[i]}";
                                list += "\n";
                            }
                        }
                        await Bot.SendTextMessageAsync(message.Chat, list);
                    }

                    if (message.Text == "да")
                    {
                        await Bot.SendTextMessageAsync(message.Chat, "Пизда", replyToMessageId: message.MessageId);
                    }

                    if (message.Text.Length <= 5 && message.Text.Length >= 2 && message.Text.Substring(message.Text.Length - 2).Contains("да"))
                    {

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
                if (message.Text == "погода" || message.Text == "/weather")
                {
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
                        var t = doc.DocumentNode.SelectSingleNode("/html/body/section/div[2]/div/div[1]/div/div[2]/div[1]/div[1]/a[1]/div/div[1]/div[3]/div[2]/span/span[1]");
                        string temp = t.InnerText;


                        if (temp.Contains("&minus;")) temp.Replace("&minus;", "-");
                        var c = doc.DocumentNode.SelectSingleNode("/html/body/section/div[2]/div/div[1]/div/div[2]/div[1]/div[1]/a[1]");
                        string cond = c.Attributes["data-text"].Value;


                        if (temp.Contains("&minus;"))
                        {
                            temp = temp.Replace("&minus;", "-");
                            temp = temp.Trim();
                            await Bot.SendTextMessageAsync(message.Chat, $"На улице сейчас.... ❄️{temp}❄️");
                        }
                        else if (Convert.ToInt32(temp) > -1 && Convert.ToInt32(temp) < 10)
                        {
                            await Bot.SendTextMessageAsync(message.Chat, $"На улице сейчас....  ✨{temp}✨");
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.Chat, $"На улице сечас....  ☀️{temp}☀️");
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


                if (message.Text == "фиона")
                {
                    await Bot.SendTextMessageAsync(message.Chat, "Привет, я Фиона, чат-бот Болота 4 :3");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADGQAD9OfCJRWWFn5c1beEFgQ");

                    await Bot.SendTextMessageAsync(message.Chat, "Мои команды:\n /status - для вызова персонального статуса  \n /game_enter - войти игру в <кто> \n /list - для просмотра всех  триггеров \n Триггер *triggger_name* - для создания нового триггера \n /weather - показать прогноз погоды на сейчас \n Задать мне вопрос - Фиона,<вопрос>?");

                }

                if (message.Text == "игроки")
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

                if(message.Text == "rtv") await Bot.SendPhotoAsync(message.Chat.Id, "https://ukr-web.org.ua/wp-content/uploads/2017/10/google.jpg", "Revolution!");

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

                if (message.Text == "бронь")
                {
                    keyboard = new InlineKeyboardMarkup(new[]
                    {

                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("2"),
                            InlineKeyboardButton.WithCallbackData("2"),
                            InlineKeyboardButton.WithCallbackData("3")
                        },

                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("3"),
                            InlineKeyboardButton.WithCallbackData("3"),
                            InlineKeyboardButton.WithCallbackData("3")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("3"),
                            InlineKeyboardButton.WithCallbackData("3"),
                            InlineKeyboardButton.WithCallbackData("3")
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Бронь парт", replyMarkup: keyboard);

                }

                if (message.Text == "статус" || message.Text == "/status")
                {
                    int c = count[people.IndexOf(message.From.Username)];
                    string r = ranks[0];
                    for (int i = 0; i < ranksc.Count; i++)
                    {
                        if (c < ranksc[i])
                        {
                            r = ranks[i];
                            i = ranksc.Count;
                        }
                    }
                    if (r == ranks[0])
                    {
                        string mes = $"\n Status: @{message.From.Username} \n Ранг: {r} \n Количесвто сообщений = {c}";
                        await Bot.SendTextMessageAsync(message.Chat.Id, mes, replyToMessageId: message.MessageId);
                        await Bot.SendPhotoAsync(message.Chat.Id, "https://i1.rgstatic.net/ii/profile.image/797915803029504-1567249360132_Q512/Glib_Tereshchenko.jpg");

                    }
                    else
                    {
                        string mes = $"\n Status: @{message.From.Username} \n Ранг: {r} \n Количесвто сообщений = {c}";
                        await Bot.SendTextMessageAsync(message.Chat.Id, mes, replyToMessageId: message.MessageId);
                    }

                }

                if (message.Text == "полезная инфа")
                {
                    Console.WriteLine(message.MigrateFromChatId);
                    if (message.ReplyToMessage.Text.Length < 1) return;
                    await Bot.ForwardMessageAsync(22, -1001100135301, message.ReplyToMessage.MessageId);
                }


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

        static void SavePeople()
        {
            using (FileStream fs = new FileStream("People.xml", FileMode.OpenOrCreate))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                fs.SetLength(0);
                serializer.Serialize(fs, people);
            }
        }

        static void LoadPeople()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<string>));
            using (FileStream fs = new FileStream("Unames.xml", FileMode.OpenOrCreate))
            {
                List<string> templist = (List<string>)xs.Deserialize(fs);
                foreach (string di in templist)
                {
                    people.Add(di);
                }
            }
        }

        static void SaveCount()
        {
            using (FileStream fs = new FileStream("Count.xml", FileMode.OpenOrCreate))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<int>));
                fs.SetLength(0);
                serializer.Serialize(fs, count);
            }
        }

        static void LoadCount()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<int>));
            using (FileStream fs = new FileStream("Count.xml", FileMode.OpenOrCreate))
            {
                List<int> templist = (List<int>)xs.Deserialize(fs);
                foreach (int di in templist)
                {
                    count.Add(di);
                }
            }
        }

    }
}

