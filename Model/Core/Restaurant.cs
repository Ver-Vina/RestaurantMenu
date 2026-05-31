using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Model.Core
{
    [XmlRoot("Restaurant")]
    [XmlInclude(typeof(HotDish))]
    [XmlInclude(typeof(Drink))]
    [XmlInclude(typeof(Dessert))]
    public class Restaurant : Establishment
    {
        public Restaurant() => Type = "Restaurant";
    }
}