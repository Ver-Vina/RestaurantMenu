using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public partial class Establishment
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Type { get; set; } // Restaurant, Cafe, CoffeeShop
        public Menu CurrentMenu { get; set; } = new Menu();
        public string FilePath { get; set; } // полный путь к файлу

    }
}