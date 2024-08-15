using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Application.Entities
{
    public class MealPlan(IList<Meal> meals)
    {
        public IList<Meal> Meals { get; } = meals ?? throw new ArgumentNullException(nameof(meals));

        public void Add(Meal meal)
        {
            Meals.Add(meal);
        }

        public void RemoveIfExists(DateOnly date, MealType mealType)
        {
            var mealToRemove = Meals.FirstOrDefault(m => m.Date == date && m.MealType == mealType);
            if (mealToRemove != null) {
                Meals.Remove(mealToRemove);
            }
        }
    }
}
