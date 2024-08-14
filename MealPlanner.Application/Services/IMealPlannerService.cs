using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Services
{
    public interface IMealPlannerService
    {
        public Task<MealPlan> GenerateNewMealPlan();
    }
}
