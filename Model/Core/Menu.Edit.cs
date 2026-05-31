using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public partial class Menu
    {
        public void AddDish(Dish dish)
        {
            Dishes.Add(dish);
        }

        public void RemoveDish(Dish dish)
        {
            Dishes.Remove(dish);
        }
    }
}