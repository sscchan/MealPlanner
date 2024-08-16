using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Services
{
    public interface IMealPlannerService
    {
        public Task<MealPlan> GenerateMealPlan(
            int numberOfDaysBeforeDishComponentsCanBeUsedAgain = 3,
            int numberOfDaysInPlan = 7,
            IList<string> priorityDishComponents = default,
            IList<string> prohibitedDishComponents = default,
            IList<string> prohibitedDishes = default);
    }
}
