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
        static Dictionary<string, string> triggers = new Dictionary<string, string>();
        static List<DataItem> tempdataitems = new List<DataItem>(triggers.Count);
        static string[] commands = new string[] { "список", "Список", "/list", "Удалить", "Триггер", "Фиона", "фиона", "Девочка", "девочка", "погода", "Погода" };
        static int counter = 38;
        static List<string> gamersId = new List<string>();
        static string[] quastions = new string[] { "кто", "у кого", "кого" };
        static string[] trues = new string[] { "Да!", "Конечно!", "Без сомнений!", "Лоол, а как же иначе!" };
        static string[] falses = new string[] { "Нет", "Конечно нет!", "Такого не можут быть!", "Фейк!" };
        static string[] bron = new string[] { "1.1", "2.1", "3.1", "1.2", "2.2", "3.2", "1.3", "2.3", "3.3" };

        static InlineKeyboardMarkup keyboard;


        static void Main(string[] args)
        {

            Bot = new TelegramBotClient("905671296:AAFcDT4qymtle-QyUne4agx14q_97mIQMXI");
            var me = Bot.GetMeAsync().Result;
            LoadTrigers();
            LoadUname();
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
            // Вывод кода стикера в консоль
            if (message.Type == MessageType.Text)
            {
                counter++;
                message.Text = message.Text.ToLower();

                if (message.Text.Contains("триггер"))
                {
                    if (message.ReplyToMessage != null)
                    {
                        if (message.ReplyToMessage.Type == MessageType.Text)
                        {
                            if (message.ReplyToMessage != null)
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
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat, "Какой-то шрэк забыл прикрепить сообщение");
                                await Bot.SendStickerAsync(message.Chat, "CAADAgADBwAD9OfCJS6YbVaPHbHaFgQ");
                            }
                        }


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


                if (message.Text.Contains("/list") || message.Text.Contains("список"))
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

                if (message.Text.Length > 3)
                {
                    if (message.Text.Substring(message.Text.Length - 2).Contains("да"))
                    {
                        if (message.Text.Length <= 5 && message.Text.Length >= 2)
                        {
                            await Bot.SendTextMessageAsync(message.Chat, "Пизда", replyToMessageId: message.MessageId);
                        }

                    }
                }


                if (triggers.ContainsKey(message.Text))
                {
                    string er = "ошибка";
                    triggers.TryGetValue(message.Text, out er);
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

                if (message.Text == "погода")
                {
                    if (counter > 40)
                    {
                        counter = 0;
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
                            Console.WriteLine(temp);
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

                    else
                    {
                        await Bot.SendTextMessageAsync(message.Chat, "Погоду можно запрашивать раз в 40 сообщений (");
                    }
                }


                if (message.Text == "фиона")
                {
                    await Bot.SendTextMessageAsync(message.Chat, "Привет, я Фиона, чат-бот Болота 4 :3");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADGQAD9OfCJRWWFn5c1beEFgQ");

                    await Bot.SendTextMessageAsync(message.Chat, "Мои команды: \n /game_enter - войти игру в <кто> \n /list - для просмотра всех  триггеров \n Триггер *triggger_name* - для создания нового триггера \n Погода - показать прогноз погоды на сейчас \n Задать мне вопрос - Фиона,<вопрос>?");
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

                if (message.Text.Contains("/game"))
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
                if (message.Text.Length > 5)
                {
                    if (message.Text.Substring(0, 5).Contains(quastions[0]) || message.Text.Substring(0, 5).Contains(quastions[1]) || message.Text.Substring(0, 5).Contains(quastions[2]))
                    {
                        if (message.Text.Contains("?"))
                        {
                            if (message != null)
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
                }


                if (message.Text.Contains("девочка"))
                {
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADKwADqWElFEZQB5e23FxJFgQ");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADyAEAArMeUCPRh9FVnGyWTRYE");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADLAADqWElFNm7GHyxzP9LFgQ");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgAD0gEAArMeUCPGE2QnmWBiEhYE");

                }

                if (message.Text.Contains("фиона,"))
                {
                    if (message.Text.Contains("?"))
                    {
                        if (message.Text.Length > 7)
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

                }

                if (message.Text == "бронь")
                {
                    keyboard = new InlineKeyboardMarkup(new[]
                    {

                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("2"),
                            InlineKeyboardButton.WithCallbackData(bron[1]),
                            InlineKeyboardButton.WithCallbackData(bron[2])
                        },

                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData(bron[3]),
                            InlineKeyboardButton.WithCallbackData(bron[4]),
                            InlineKeyboardButton.WithCallbackData(bron[5])
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData(bron[6]),
                            InlineKeyboardButton.WithCallbackData(bron[7]),
                            InlineKeyboardButton.WithCallbackData(bron[8])
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Бронь парт", replyMarkup: keyboard);

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



    }
}

