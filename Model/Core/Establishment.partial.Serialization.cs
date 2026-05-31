using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public partial class Establishment
    {
        public List<Dish> Select(Type dishType)
        {
            return CurrentMenu.Dishes.Where(d => d.GetType() == dishType).ToList();
        }

        public List<Dish> Select(string categoryName)
        {
            return CurrentMenu.Dishes.Where(d => d.Category == categoryName).ToList();
        }
    }
}