﻿using Bot_Fiona;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace botfiona
{
  public class RankManager
  {
    private Dictionary<int, string> numRanks;
    private Dictionary<string, string> ranksPics;
    private TelegramBotClient Bot = Program.Bot;
    private MessageEventArgs m = Program.ames;
    public RankManager()
    {

      numRanks = new Dictionary<int, string>()
            {
                {0, "Дантес" }, { 150, "Глеб" }, { 800, "Мудрец" }, { 2000, "Мамонт" },
                { 5000, "Харьковчанин" }, { 10000, "Хнурэшник" },
                { 25000, "Языковая владыка" }, { 88888, "Болотный барт" },
                { 100000, "Салтовчанен" }
            };

      ranksPics = new Dictionary<string, string>()
            {
              {"Дантес", "http://pushkin.ellink.ru/2018/pushkin/images/fullscreen/pushkin.jpg"}, { "Глеб", "https://i1.rgstatic.net/ii/profile.image/797915803029504-1567249360132_Q512/Glib_Tereshchenko.jpg" } ,
              {"Мудрец", "https://i1.rgstatic.net/ii/profile.image/797915803029504-1567249360132_Q512/Glib_Tereshchenko.jpg" }, {"Мамонт", "https://ichef.bbci.co.uk/news/410/cpsprodpb/17D22/production/_90607579_3274gazt.jpg"},
              {"Харьковчанин", "http://bardak.kharkov.ua/wp-content/uploads/2017/09/08-09-01-1.jpg"}, {"Хнурэшник", "https://telegra.ph/file/5e7b018be301866334cbd.jpg"},
              {"Языковая владыка", "https://nure.ua/wp-content/uploads/Employees_photo/LitvinAG.jpg" }
            };

    }

    public string GetFormattedString(int userMsgCount, string userName)
    {
      string rank = "";
      for (int i = 0; i < numRanks.Count; i++)
      {
        if (userMsgCount > numRanks.Keys.ElementAt(i))
          rank = numRanks.Values.ElementAt(i);
      }
      return string.Format("Юзер: @{0} \nРанг: {1} \nКол-во сообщений: {2}", userName, rank, userMsgCount);
    }

    public bool CountExists(int msgUser)
    {
      if (numRanks.ContainsKey(msgUser)) return true;
      else return false;
    }

    public string GetRank(int index)
    {
      return numRanks[index];
    }

    public string GetPic(int mescount)
    {
      string rank = "";
      for (int i = 0; i < numRanks.Count; i ++)
      {
        if (mescount >= numRanks.ElementAt(i).Key && mescount < numRanks.ElementAt(i + 1).Key)
        {
          rank = numRanks.ElementAt(i).Value;
          break;
        }
      }
      return ranksPics[rank];
    }

    public void TopRank()
    {
      if(TextManager.mesCount.Count == 0)
      {
        TextManager man = new TextManager();
        man.LoadMes();
      }
      var items = from pair in TextManager.mesCount
                  orderby pair.Value descending
                  select pair;
      string top = "Топ 10:\n";
      List<string> tops = new List<string>();
      int counter = 1;
      foreach (KeyValuePair<string, int> pair in items)
      {
        if (counter > 10) break;
          top += $"\n({counter}) " + pair.Key + " - " + pair.Value + "\n";
        counter++;
      }
      Bot.SendTextMessageAsync(m.Message.Chat.Id, top);
    }

    public async void Status()
    {

      TextManager textm = new TextManager();
      var message = m.Message;
      Inventory inv = new Inventory();
      string w = "\nОружие: " + inv.GetWeapon(message.From.Username);
      string msg = GetFormattedString(TextManager.mesCount[message.From.Username], message.From.Username) + $"\nКоличество побед:  {Battle.GetWins(message.From.Username)}" + w;
      await Bot.SendPhotoAsync(message.Chat.Id, GetPic(TextManager.mesCount[message.From.Username]), msg, replyToMessageId: message.MessageId);
    }

  }

}