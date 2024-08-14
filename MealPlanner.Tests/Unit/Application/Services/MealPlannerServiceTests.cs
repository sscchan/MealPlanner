using MealPlanner.Application.Entities;
using MealPlanner.Application.Services;
using MealPlanner.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Act
            MealPlan mealPlan = await _mealPlannerService.GenerateNewMealPlan();

            // Assert
            Assert.IsTrue(mealPlan.Meals.Any());
        }
    }
}
