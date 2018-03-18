using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NTTCP
{
    [Serializable()]
    public class User
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("age")]
        public int Age { get; set; }

        public User()
            : this("", -1)
        {
        }

        public User(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return Name + "|" + Age;
        }
    }
}
