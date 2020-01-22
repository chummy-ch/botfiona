using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace botfiona
{
  class PersonManager
  {
    private List<Person> people;


    public PersonManager()
    {
      people = new List<Person>();
      LoadPeople();
    }


    public void AddPerson(Person person)
    {
      people.Add(person);
      SavePeople();
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
