using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Repositories
{
    public interface IMealPlanRepository
    {
        /// <summary>
        /// Returns the MealPlan entries in the repository
        /// </summary>
        public Task<IEnumerable<MealPlan>> Get();
    }
}
