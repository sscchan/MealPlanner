using MealPlanner.Application.Entities;
using MealPlanner.Application.Services;
using MealPlanner.Infrastructure.Repositories;

namespace MealPlanner.Tests.Unit.Application.Services
{
    [TestClass]
    public class MealPlannerServiceTests
    {
        private CsvFileDishRepository _csvFileDishRepository;
        private MealPlannerService _mealPlannerService;

        [TestInitialize]
        public void Initialize()
        {
            _csvFileDishRepository = new CsvFileDishRepository(csvDataFilePathOverride: string.Empty);
            _mealPlannerService = new MealPlannerService(_csvFileDishRepository);
        }

        [TestMethod]
        public async Task GenerateMealPlan()
        {
            // Generate large number of new meal plans to test for possible scenarios where a meal plan cannot be created
            // due the not having dishes with diverse enough components in the Dishes database.
            for (int i = 0; i < 1000; i++)
            {
                // Act
                MealPlan mealPlan = await _mealPlannerService.GenerateMealPlan();

                // Assert
                Assert.IsTrue(mealPlan.Meals.Any());
            }
        }

        [TestMethod]
        public async Task GeneralMealPlanIncludesPriorityComponent()
        {
            // Act
            MealPlan mealPlan = await _mealPlannerService.GenerateMealPlan(
                priorityDishComponents: ["Rice", "Chicken", "Carrot", "Apple"]);

            // Assert
            var dishes = mealPlan.Meals.SelectMany(m => m.Dishes);

            Assert.IsTrue(dishes
                .SelectMany(d => d.CarbohydrateComponents)
                .Any(pc => pc == "Rice"));

            Assert.IsTrue(dishes
                .SelectMany(d => d.ProteinComponents)
                .Any(pc => pc == "Chicken"));

            Assert.IsTrue(dishes
                .SelectMany(d => d.VegetableComponents)
                .Any(pc => pc == "Carrot"));

            Assert.IsTrue(dishes
                .SelectMany(d => d.FruitComponents)
                .Any(pc => pc == "Apple"));
        }

        [TestMethod]
        public async Task GeneralMealPlanDoesNotIncludeProhibitedDishComponents()
        {
            // Act
            MealPlan mealPlan = await _mealPlannerService.GenerateMealPlan(
                prohibitedDishComponents: ["Rice", "Chicken", "Carrot", "Apple"]);

            // Assert
            var dishes = mealPlan.Meals.SelectMany(m => m.Dishes);

            Assert.IsFalse(dishes
                .SelectMany(d => d.CarbohydrateComponents)
                .Any(pc => pc == "Rice"));

            Assert.IsFalse(dishes
                .SelectMany(d => d.ProteinComponents)
                .Any(pc => pc == "Chicken"));

            Assert.IsFalse(dishes
                .SelectMany(d => d.VegetableComponents)
                .Any(pc => pc == "Carrot"));

            Assert.IsFalse(dishes
                .SelectMany(d => d.FruitComponents)
                .Any(pc => pc == "Apple"));
        }

    }
}
