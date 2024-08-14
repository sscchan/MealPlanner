using MealPlanner.Application.Entities;
using MealPlanner.Application.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Application.Services
{
    public class MealPlannerService : IMealPlannerService
    {
        private IDishRepository _dishRepository;

        private Random _randomNumerGenerator;

        public MealPlannerService(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
            _randomNumerGenerator = new Random();
        }
        public async Task<MealPlan> GenerateNewMealPlan()
        {
            MealPlan mealPlan = new MealPlan(new List<Meal>());
            var availableDishes = await _dishRepository.GetAll().ToListAsync();

            for (int i = 0; i < 7; i++)
            {
                var dishes = new List<Dish>();

                // Do not allow repeats of compoents that has been planned in the last 3 days
                var excludedDishes = mealPlan.Meals
                    .TakeLast(3)
                    .SelectMany(m => m.Dishes)
                    .ToList();

                // Predicate that tests if a potential Dish candidate has any ingredients that are already excludedDishes
                Func<Dish, bool> hasNoRepeatComponents = (dish) =>
                {
                    return
                        dish.CarbohydrateComponents.All(cc => !excludedDishes.SelectMany(d => d.CarbohydrateComponents).Contains(cc)) &&
                        dish.ProteinComponents.All(pc => !excludedDishes.SelectMany(d => d.ProteinComponents).Contains(pc)) &&
                        dish.VegetableComponents.All(vc => !excludedDishes.SelectMany(d => d.VegetableComponents).Contains(vc)) &&
                        dish.FruitComponents.All(fc => !excludedDishes.SelectMany(d => d.FruitComponents).Contains(fc));
                };

                // Select Dishes with Carbohydrate
                var dishesWithCarbohydrate = availableDishes
                    .Where(d => d.CarbohydrateComponents.Any())
                    .Where(hasNoRepeatComponents)
                    .ToList();
                var dishWithCarbohydrate = dishesWithCarbohydrate[_randomNumerGenerator.Next(0, dishesWithCarbohydrate.Count - 1)];

                dishes.Add(dishWithCarbohydrate);
                excludedDishes.Add(dishWithCarbohydrate);

                // Select Dishes with Protein if required
                if (dishes.All(d => !d.ProteinComponents.Any()))
                {
                    var dishesWithProtein = availableDishes
                            .Where(d => d.ProteinComponents.Any()) // Contains Protein
                            .Where(d => hasNoRepeatComponents(d))
                            .ToList();
                    var dishWithProtein = dishesWithProtein[_randomNumerGenerator.Next(0, dishesWithProtein.Count - 1)];

                    dishes.Add(dishWithProtein);
                    excludedDishes.Add(dishWithProtein);
                }

                if (dishes.All(d => !d.VegetableComponents.Any()))
                {
                    // Select Dishes with Vegetables if required
                    var dishesWithVegetables = availableDishes
                                .Where(d => d.VegetableComponents.Any()) // Contains Vegetables
                                .Where(d => hasNoRepeatComponents(d))
                                .ToList();

                    var dishWithVegetables = dishesWithVegetables[_randomNumerGenerator.Next(0, dishesWithVegetables.Count - 1)];

                    dishes.Add(dishWithVegetables);
                    excludedDishes.Add(dishWithVegetables);
                }

                // Select Dishes with Fruits if required
                if (dishes.All(d => !d.FruitComponents.Any()))
                {
                    var dishesWithFruit = availableDishes
                        .Where(d => d.FruitComponents.Any()) // Contains Fruit
                        .Where(d => hasNoRepeatComponents(d))
                        .ToList();
                    var dishWithFruit = dishesWithFruit[_randomNumerGenerator.Next(0, dishesWithFruit.Count - 1)];

                    dishes.Add(dishWithFruit);
                    excludedDishes.Add(dishWithFruit);
                }

                var meal = new Meal(
                    DateOnly.FromDateTime(DateTime.Today.AddDays(i)),
                    MealType.DINNER,
                    dishes);

                mealPlan.Add(meal);
            }
            return mealPlan;
        }
    }
}
