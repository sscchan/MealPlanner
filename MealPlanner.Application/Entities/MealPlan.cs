using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Application.Entities
{
    public class MealPlan(IEnumerable<Meal> meals)
    {
        IEnumerable<Meal> Meals { get; } = meals ?? throw new ArgumentNullException(nameof(meals));
    }
}
