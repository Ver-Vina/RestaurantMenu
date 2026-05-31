using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Core
{
    public interface ISeasonalMenu
    {
        bool IsSeasonal { get; set; }
        List<Dish> SeasonalDishes { get; set; }
        void AddSeasonalDish(Dish dish);
        void RemoveSeasonalDish(Dish dish);
    }
}