using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using System.Threading;
using HtmlAgilityPack;




namespace botfiona
{
    class Program
    {
        static TelegramBotClient Bot;
        static Dictionary<string, string> triggers = new Dictionary<string, string>();
        static List<DataItem> tempdataitems = new List<DataItem>(triggers.Count);
        static string[] commands = new string[] { "список", "Список", "/list", "Удалить", "Триггер", "Фиона", "фиона", "Девочка", "девочка", "погода", "Погода" };
        static int counter = 38;







        static void Main(string[] args)
        {
            Bot = new TelegramBotClient("905671296:AAFcDT4qymtle-QyUne4agx14q_97mIQMXI");
            var me = Bot.GetMeAsync().Result;
            Bot.OnMessage += Get_Mes;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
            LoadTrigers();
            foreach (string key in triggers.Keys)

            {

                tempdataitems.Add(new DataItem(key, triggers[key].ToString()));

            }
            Console.ReadKey();
        }

        private static void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static async void Get_Mes(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            // Вывод кода стикера в консоль
            /*if (message.Type == MessageType.Sticker)
            {
                var index = message.Sticker.FileId;
                Console.WriteLine(index);
            }*/
            if (message.Type == MessageType.Text)
            {
                counter++;

                if (message.Text.Contains("Триггер"))
                {
                    if (message.ReplyToMessage != null)
                    {
                        if (triggers.ContainsKey(message.Text.Split('*')[1]))
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


                if (message.Text.Contains("Удалить"))
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


                if (message.Text.Contains("/list") || message.Text.Contains("Список") || message.Text.Contains("список"))
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
                        else
                        {
                            list += "\n";
                            list += $"{triggers.Keys.ToList()[i]} - {triggers.Values.ToList()[i]}";
                            list += "\n";
                        }
                    }
                    await Bot.SendTextMessageAsync(message.Chat, list);
                }

                if (message.Text == "да" || message.Text == "Да")
                {
                    await Bot.ForwardMessageAsync(message.Chat, message.Chat, message.MessageId);
                    await Bot.SendTextMessageAsync(message.Chat, "Пизда");
                }

                if (message.Text.Length > 3)
                {
                    if (message.Text.Substring(message.Text.Length - 2).Contains("да"))
                    {
                        if (message.Text.Length <= 5 && message.Text.Length >= 2)
                        {
                            await Bot.ForwardMessageAsync(message.Chat, message.Chat, message.MessageId);
                            await Bot.SendTextMessageAsync(message.Chat, "Пизда");
                        }

                    }
                }
                

                if (triggers.ContainsKey(message.Text) == true)
                {

                    string er = "ошибка";
                    triggers.TryGetValue(message.Text, out er);
                    if (er.Contains("CAA"))
                    {
                        await Bot.SendStickerAsync(message.Chat, triggers[message.Text]);
                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(message.Chat, er);
                    }
                }

                if(message.Text == "погода" || message.Text == "Погода")
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
                            else await Bot.SendTextMessageAsync(message.Chat, $"{cond}");
                        }
                        else if (Convert.ToInt32(temp) > -1 && Convert.ToInt32(temp) < 10)
                        {
                            await Bot.SendTextMessageAsync(message.Chat, $"На улице сейчас....  ✨{temp}✨");
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
                            else await Bot.SendTextMessageAsync(message.Chat, $"{cond}");
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.Chat, $"На улице сечас....  ☀️{temp}☀️");
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
                            else await Bot.SendTextMessageAsync(message.Chat, $"{cond}");
                            
                        }

                    }
                    
                    else
                    {
                        await Bot.SendTextMessageAsync(message.Chat, "Погоду можно запрашивать раз в 40 сообщений (");
                    }
                }
                    



                if (message.Text == "Фиона")
                {
                    await Bot.SendTextMessageAsync(message.Chat, "Привет, я Фиона, чат-бот Болота 4 :3");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADGQAD9OfCJRWWFn5c1beEFgQ");

                    await Bot.SendTextMessageAsync(message.Chat, "Мои команды:\n /list - для просмотра всех  триггеров \n Триггер *triggger_name* - для создания нового триггера \n Погода-показать прогноз погоды на сейчас");
                }

                if (message.Text == "фиона")
                {
                    string name = message.From.FirstName;
                    string name1 = name.ToLower();
                    await Bot.SendTextMessageAsync(message.Chat, $"{name1}, не очень приятно, да? (o-_-o)");
                }

                if (message.Text.Contains("Девочка") || message.Text.Contains("девочка"))
                {
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADKwADqWElFEZQB5e23FxJFgQ");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADyAEAArMeUCPRh9FVnGyWTRYE");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADLAADqWElFNm7GHyxzP9LFgQ");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgAD0gEAArMeUCPGE2QnmWBiEhYE");

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
                List<DataItem> templist = (List<DataItem>)xs.Deserialize(fs);
                foreach (DataItem di in templist)
                {
                    triggers.Add(di.Key, di.Value);
                }
            }

            Program.triggers = triggers;
        }

        
    }
}

