using MealPlanner.Application.Entities;
using MealPlanner.Application.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Application.Services
{
    public class MealPlannerService : IMealPlannerService
    {
        private IDishRepository _dishRepository;

        private Random _randomNumberGenerator;

        public MealPlannerService(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
            _randomNumberGenerator = new Random();
        }
        public async Task<MealPlan> GenerateNewMealPlan(uint numberOfDaysToLookBackwardsForRepeats = 3, uint numberOfDaysToForwardPlan = 7, IList<string> initialComponentsToAvoidDuplicates = default)
        {
            MealPlan mealPlan = new MealPlan(new List<Meal>());

            // Create a custom Dish that contains all of the components that is to be avoid in the beginning of the planning process
            if (initialComponentsToAvoidDuplicates != null && initialComponentsToAvoidDuplicates.Count() > 0)
            {
                var previousMeal = new Meal(
                    DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                    MealType.DINNER,
                    // All component types have the avoided components assigned to it. This logic works as long as component names are unique across component types.
                    [new("PreviousMealDishes", initialComponentsToAvoidDuplicates, initialComponentsToAvoidDuplicates, initialComponentsToAvoidDuplicates, initialComponentsToAvoidDuplicates)]);
                mealPlan.Add(previousMeal);
            }

            var availableDishes = await _dishRepository.GetAll().ToListAsync();

            for (int i = 0; i < numberOfDaysToForwardPlan; i++)
            {
                var dishes = new List<Dish>();

                // Do not allow repeats of dish components looking backwards
                var excludedDishes = mealPlan.Meals
                    .TakeLast((int)numberOfDaysToLookBackwardsForRepeats)
                    .SelectMany(m => m.Dishes)
                    .ToList();

                // Predicate that tests if a potential Dish candidate has any ingredients that are part of any excludedDishes
                Func<Dish, bool> notContainAlreadyUsedComponents = (dish) =>
                {
                    return
                        dish.CarbohydrateComponents.All(cc => !excludedDishes.SelectMany(d => d.CarbohydrateComponents).Contains(cc)) &&
                        dish.ProteinComponents.All(pc => !excludedDishes.SelectMany(d => d.ProteinComponents).Contains(pc)) &&
                        dish.VegetableComponents.All(vc => !excludedDishes.SelectMany(d => d.VegetableComponents).Contains(vc)) &&
                        dish.FruitComponents.All(fc => !excludedDishes.SelectMany(d => d.FruitComponents).Contains(fc));
                };

                // Predicate taht tests if a dish does not contain carbohydrate components 
                Func<Dish, bool> containNoCarbohydrates = (dish) => !dish.CarbohydrateComponents.Any();

                // Function to pick a random dish from a list.
                Func<IList<Dish>, Dish> pickRandom = (d) => d[_randomNumberGenerator.Next(0, d.Count - 1)];

                // Select Dishes with Carbohydrate
                var dishesWithCarbohydrate = availableDishes
                    .Where(d => d.CarbohydrateComponents.Any())
                    .Where(notContainAlreadyUsedComponents)
                    .ToList();
                var dishWithCarbohydrate = pickRandom(dishesWithCarbohydrate);

                dishes.Add(dishWithCarbohydrate);
                excludedDishes.Add(dishWithCarbohydrate);

                // Select Dishes with Protein if required
                if (dishes.All(d => !d.ProteinComponents.Any()))
                {
                    var dishesWithProtein = availableDishes
                            .Where(d => d.ProteinComponents.Any()) // Contains Protein
                            .Where(containNoCarbohydrates) // Does NOT contains Carbohydrate to preserve options for future dishes (due to the low number of carbohydrate options)
                            .Where(notContainAlreadyUsedComponents)
                            .ToList();
                    var dishWithProtein = pickRandom(dishesWithProtein);

                    dishes.Add(dishWithProtein);
                    excludedDishes.Add(dishWithProtein);
                }

                if (dishes.All(d => !d.VegetableComponents.Any()))
                {
                    // Select Dishes with Vegetables if required
                    var dishesWithVegetables = availableDishes
                                .Where(d => d.VegetableComponents.Any()) // Contains Vegetables
                                .Where(containNoCarbohydrates) // Does NOT contains Carbohydrate to preserve options for future dishes (due to the low number of carbohydrate options)
                                .Where(notContainAlreadyUsedComponents)
                                .ToList();

                    var dishWithVegetables = pickRandom(dishesWithVegetables);

                    dishes.Add(dishWithVegetables);
                    excludedDishes.Add(dishWithVegetables);
                }

                // Select Dishes with Fruits if required
                if (dishes.All(d => !d.FruitComponents.Any()))
                {
                    var dishesWithFruit = availableDishes
                        .Where(d => d.FruitComponents.Any()) // Contains Fruit
                        .Where(containNoCarbohydrates) // Does NOT contains Carbohydrate to preserve options for future dishes (due to the low number of carbohydrate options)
                        .Where(notContainAlreadyUsedComponents)
                        .ToList();
                    var dishWithFruit = pickRandom(dishesWithFruit);

                    dishes.Add(dishWithFruit);
                    excludedDishes.Add(dishWithFruit);
                }

                var meal = new Meal(
                    DateOnly.FromDateTime(DateTime.Today.AddDays(i)),
                    MealType.DINNER,
                    dishes);

                mealPlan.Add(meal);
            }

            // Remove entry used to exclude dish components in the early part of the planning
            mealPlan.RemoveIfExists(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), MealType.DINNER);

            return mealPlan;
        }
    }
}
