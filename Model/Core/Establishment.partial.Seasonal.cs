using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public partial class Establishment : ISeasonalMenu
    {
        public bool IsSeasonal { get; set; }
        public List<Dish> SeasonalDishes { get; set; } = new List<Dish>();

        public void AddSeasonalDish(Dish dish)
        {
            SeasonalDishes.Add(dish);
        }

        public void RemoveSeasonalDish(Dish dish)
        {
            SeasonalDishes.Remove(dish);
        }
    }
}