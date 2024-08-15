using MealPlanner.Application.Entities;
namespace MealPlanner.Application.Services
{
    public interface IDishDataManagementService
    {
        public IAsyncEnumerable<Dish> GetDishes();

        public Task<IDictionary<string, IEnumerable<string>>> GetDishComponents();
    }
}
