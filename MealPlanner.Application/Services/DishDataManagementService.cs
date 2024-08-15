using MealPlanner.Application.Entities;
using MealPlanner.Application.Repositories;

namespace MealPlanner.Application.Services
{
    public class DishDataManagementService : IDishDataManagementService
    {
        private IDishRepository _dishRepository;

        public DishDataManagementService(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public IAsyncEnumerable<Dish> GetDishes()
        {
            return _dishRepository.GetAll();
        }

        public async Task<IDictionary<string, IEnumerable<string>>> GetDishComponents()
        {
            var dishes = await _dishRepository.GetAll().ToListAsync();

            var dishComponents = new Dictionary<string, IEnumerable<string>>()
            {
                { "Carbohydrates", dishes.SelectMany(d => d.CarbohydrateComponents).ToHashSet().ToList() },
                { "Proteins", dishes.SelectMany(d => d.ProteinComponents).ToHashSet().ToList() },
                { "Vegetables", dishes.SelectMany(d => d.VegetableComponents).ToHashSet().ToList() },
                { "Fruits", dishes.SelectMany(d => d.FruitComponents).ToHashSet().ToList() },
            };

            return dishComponents;
        }
    }
}
