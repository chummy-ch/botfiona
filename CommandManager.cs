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
/*    TelegramBotClient bot = Program.Bot;
*/    private List<string> commands = new List<string>() { "/weather", "/status", "/battle", "/stopb", "/list" };
    public CommandManager()
    {

    }

    public void CheckCommand(string command)
    {
      if (command.Contains("@Fionaa_bot")) command = command.Replace("@Fionaa_bot", "");
      if(commands.Contains(command))
      {
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
          case "/stopb":
            Battle battle = new Battle();
            Program.online = false;
            battle.StopBattle();
            break;
          case "/list":
            ListGen listGen = new ListGen();
            listGen.GetList();
            break;
        }
      }
    }

  }
}
