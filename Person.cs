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
  class Person
  {
    public int Wins { get; set; }
    public string Name { get; set; }
    public int Mes { get; set; }
    public string Weapon { get; set; }

    public List<Person> people = new List<Person>();

    public Person(string name, int mes, int wins, string weapon)
    {
      this.Name = name;
      this.Mes = mes;
      this.Wins = wins;
      this.Weapon = weapon;

    }

    public Person()
    {

    }


  }
}
