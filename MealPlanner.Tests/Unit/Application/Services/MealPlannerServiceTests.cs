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
        public async Task GenerateNewMealPlan()
        {
            // Generate large number of new meal plans to test for possible scenarios where a meal plan cannot be created
            // due the not having dishes with diverse enough components in the Dishes database.
            for (int i = 0; i < 1000; i++)
            {
                // Act
                MealPlan mealPlan = await _mealPlannerService.GenerateNewMealPlan();

                // Assert
                Assert.IsTrue(mealPlan.Meals.Any());
            }
        }
    }
}
