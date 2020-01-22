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

    public Person(string name, int mes, int wins)
    {
      this.Name = name;
      this.Mes = mes;
      this.Wins = wins;
    }

    public Person()
    {

    }

    public  void MakePerson(string UName, int mesC, int winsc)
    {
      Person person = new Person( UName,  mesC, winsc );
      
    }

  }
}
