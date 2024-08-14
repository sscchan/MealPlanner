using MealPlanner.Infrastructure.Repositories;

namespace MealPlanner.Tests.Unit.Infrastructure.Repositories
{
    [TestClass]
    public class CsvFileDishRepositoryTests
    {
        private CsvFileDishRepository _csvFileDishRepository;

        [TestInitialize]
        public void Initialize()
        {
            // Instantiate a repository that reads from the default CSV file path
            _csvFileDishRepository = new CsvFileDishRepository(csvDataFilePathOverride: string.Empty);
        }

        [TestMethod]
        public async Task GetAll()
        {
            // Act
            var dishes = _csvFileDishRepository.GetAll();

            // Assert
            var dishCount = 0;

            await foreach (var dish in dishes)
            {
                dishCount++;

                Assert.IsFalse(string.IsNullOrEmpty(dish.Name));
                Assert.IsNotNull(dish.CarbohydrateComponents);
                Assert.IsNotNull(dish.ProteinComponents);
                Assert.IsNotNull(dish.VegetableComponents);
                Assert.IsNotNull(dish.FruitComponents);
            }

            Assert.IsTrue(dishCount > 0);
        }
    }
}
