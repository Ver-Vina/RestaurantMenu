using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public partial class Menu : IMenu
    {
        public List<Dish> Dishes { get; set; } = new List<Dish>();
    }
}