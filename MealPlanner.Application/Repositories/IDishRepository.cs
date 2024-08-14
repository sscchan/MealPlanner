using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Repositories
{
    public interface IDishRepository
    {
        /// <summary>
        /// Returns all Dish entries in the repository
        /// </summary>
        public IAsyncEnumerable<Dish> GetAll();
    }
}
