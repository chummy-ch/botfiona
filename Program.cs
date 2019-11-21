using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;



namespace botfiona
{
    class Program
    {
        static TelegramBotClient Bot;
        static Dictionary<string, string> triggers = new Dictionary<string, string>();
        static List<DataItem> tempdataitems = new List<DataItem>(triggers.Count);








        static void Main(string[] args)
        {
            Bot = new TelegramBotClient("905671296:AAFcDT4qymtle-QyUne4agx14q_97mIQMXI");
            var me = Bot.GetMeAsync().Result;
            Console.WriteLine(me.FirstName);
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
            /*if (message.Type == MessageType.Sticker)
            {
                var index = message.Sticker.FileId;
                Console.WriteLine(index);
            }*/
            if (message.Type == MessageType.Text)
            {
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
                                string key = message.Text.Split('*')[1];
                                triggers.Add(key, message.ReplyToMessage.Text);
                                SaveTriggers();
                                await Bot.SendTextMessageAsync(message.Chat, "Триггер создан!");
                                await Bot.SendStickerAsync(message.Chat, "CAADAgADBgADCsj5K2VYWFJWqNsGFgQ");

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
                        else
                        {
                            list += "\n";
                            list += $"{triggers.Keys.ToList()[i]} - {triggers.Values.ToList()[i]}";
                            list += "\n";
                        }
                    }

                    Console.WriteLine(list.Length);
                    await Bot.SendTextMessageAsync(message.Chat, list);
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


                if (message.Text == "Фиона")
                {
                    await Bot.SendTextMessageAsync(message.Chat, "Привет, я Фиона, чат-бот Болота 4 :3");
                    await Bot.SendStickerAsync(message.Chat, "CAADAgADGQAD9OfCJRWWFn5c1beEFgQ");

                    await Bot.SendTextMessageAsync(message.Chat, "Мои команды:\n /list - для просмотра всех  триггеров \n Триггер *triggger_name* - для создания нового триггера");
                }

                if (message.Text == "фиона")
                {
                    string name = message.From.FirstName;
                    string name1 = name.ToLower();
                    await Bot.SendTextMessageAsync(message.Chat, $"{name1}, не очень приятно, да? (o-_-o)");
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

