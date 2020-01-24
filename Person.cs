using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;

namespace botfiona
{
  [Serializable]
  class Person
  {
    public int Wins { get; set; }
    public string Name { get; set; }
    public int Mes { get; set; }
/*    public string Weapon { get; set; }
*/
    public List<Person> people = new List<Person>();

    public Person(string name, int mes, int wins/*, string weapon*/)
    {
      this.Name = name;
      this.Mes = mes;
      this.Wins = wins;
/*      this.Weapon = weapon;
*/      
    }

    public Person()
    {

    }

    public  void MakePerson(string UName, int mesC, int winsc/*, string weapons*/)
    {
      LoadPeople();
      Person person = new Person(UName, mesC, winsc);

    }

    public void SavePeople()
    {
      using (StreamWriter writer = File.CreateText("persons.txt"))
      {
        var settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        JsonSerializer.Create(settings).Serialize(writer, people);
      }
    }

    public void LoadPeople()
    {
      if (!File.Exists("persons.txt")) return;
      string json = File.ReadAllText("persons.txt");
      people = new JavaScriptSerializer().Deserialize<List<Person>>(json);
    }

  }
}
