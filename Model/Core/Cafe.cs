using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Model.Core
{
    [XmlRoot("Cafe")]
    [XmlInclude(typeof(HotDish))]
    [XmlInclude(typeof(Drink))]
    [XmlInclude(typeof(Dessert))]
    public class Cafe : Establishment
    {
        public Cafe() => Type = "Cafe";
    }
}
