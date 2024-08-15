using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Services
{
    public interface IMealPlannerService
    {
        public Task<MealPlan> GenerateNewMealPlan(uint numberOfDaysToLookBackwardsForRepeats = 3, uint numberOfDaysToForwardPlan = 7, IList<string> initialComponentsToAvoidDuplicates = default);
    }
}
