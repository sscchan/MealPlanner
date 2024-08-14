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
    }
}
