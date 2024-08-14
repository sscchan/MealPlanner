using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Application.Entities
{
    public class Meal(DateOnly date, MealType mealType, Dish dish)
    {
        public DateOnly Date { get; } = date;
        public MealType MealType { get; } = mealType;
        public Dish Dish { get; } = dish ?? throw new ArgumentNullException(nameof(dish));
    }
}
