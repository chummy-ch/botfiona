using Bot_Fiona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;


namespace botfiona
{

  public class CommandManager
  {
    private List<string> commands = new List<string>() { "/weather", "/status", "/battle", "/stopb", "/list", "/roulette", "/inv", "/ranktop", "/battle", "/players", "/game", "/equip" };
    public MessageEventArgs e;

    public CommandManager(MessageEventArgs e)
    {
      this.e = e;
    }

    public void CheckCommand(string command)
    {
      if (command.Contains("@Fionaa_bot")) command = command.Replace("@Fionaa_bot", "");
      if (!commands.Contains(command)) return;
      switch (command)
      {
        case "/weather":
          Weather weather = new Weather();
          weather.GetWeather();
          break;
        case "/status":
          RankManager rank = new RankManager();
          rank.Status();
          break;
        case "/battle":
          if (BattleManager.online == true) return;
          BattleManager battlemanager = new BattleManager(e);
          battlemanager.PreBattle();
          break;
        case "/stopb":
          Battle.StopBattle();
          break;
        case "/list":
          ListGen listGen = new ListGen();
          listGen.GetList();
          break;
        case "/roulette":
          Roulette roulette = new Roulette();
          roulette.CreateRoll();
          break;
        case "/inv":
          Inventory inv = new Inventory();
          inv.GetInventory();
          break;
        case "/ranktop":
          RankManager rankm = new RankManager();
          rankm.TopRank();
          break;
        case "/players":
          MiniGames min2 = new MiniGames(e.Message);
          min2.GetPlayers();
          break;
        case "/game":
          MiniGames min = new MiniGames(e.Message);
          min.AddPlayer();
          break;
        case "/equip":
          Console.WriteLine("CM");
          Inventory i = new Inventory();
          i.Equip(e.Message.From.Username);
          break;
      }
    }

  }
}
