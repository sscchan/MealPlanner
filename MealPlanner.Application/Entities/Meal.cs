using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Application.Entities
{
    public class Meal(DateOnly date, MealType mealType, IEnumerable<Dish> dishes)
    {
        public DateOnly Date { get; } = date;
        public MealType MealType { get; } = mealType;
        public IEnumerable<Dish> Dishes { get; } = dishes ?? throw new ArgumentNullException(nameof(dishes));
    }
}
