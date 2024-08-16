using MealPlanner.Application.Entities;
using MealPlanner.Application.Repositories;

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

        public async Task<MealPlan> GenerateMealPlan(
            int numberOfDaysBeforeDishComponentsCanBeUsedAgain = 3,
            int numberOfDaysInPlan = 7,
            IList<string> priorityDishComponents = default,
            IList<string> prohibitedDishComponents = default,
            IList<string> prohibitedDishes = default)
        {
            // Assign empty list for optional ist parameters
            priorityDishComponents ??= new List<string>();
            prohibitedDishComponents ??= new List<string>();
            prohibitedDishes ??= new List<string>();

            var allDishes = await _dishRepository.GetAll().ToListAsync();
            var availableDishes = allDishes
                .Where(d => !prohibitedDishes.Contains(d.Name))
                .Where(d => !prohibitedDishComponents.Intersect(d.CarbohydrateComponents).Any())
                .Where(d => !prohibitedDishComponents.Intersect(d.ProteinComponents).Any())
                .Where(d => !prohibitedDishComponents.Intersect(d.VegetableComponents).Any())
                .Where(d => !prohibitedDishComponents.Intersect(d.FruitComponents).Any())
                .ToList();

            MealPlan mealPlan = new MealPlan(new List<Meal>());

            for (int i = 0; i < numberOfDaysInPlan; i++)
            {
                var currentDishes = new List<Dish>();

                var previousDishesWithinLookbackPeriod = mealPlan.Meals
                    .TakeLast(numberOfDaysBeforeDishComponentsCanBeUsedAgain)
                    .SelectMany(m => m.Dishes)
                    .ToList();

                var previousDishComponentsWithinLookbackPeriod = 
                    previousDishesWithinLookbackPeriod.SelectMany(d => d.CarbohydrateComponents).Concat(
                    previousDishesWithinLookbackPeriod.SelectMany(d => d.ProteinComponents)).Concat(
                    previousDishesWithinLookbackPeriod.SelectMany(d => d.VegetableComponents)).Concat(
                    previousDishesWithinLookbackPeriod.SelectMany(d => d.FruitComponents)).ToList();

                // Create a copy, as more components will be added to it as we plan each dish in a meal in order to avoid duplicated components within a meal.
                var dishComponentsToAvoidRepeating = new List<string>(previousDishComponentsWithinLookbackPeriod);

                // Predicate to check if a dish contains any components that we wish to avoid repeats of.
                Func<Dish, bool> doesNotContainRepeatingComponents = (dish) =>
                        !dishComponentsToAvoidRepeating.Intersect(dish.CarbohydrateComponents).Any() &&
                        !dishComponentsToAvoidRepeating.Intersect(dish.ProteinComponents).Any() &&
                        !dishComponentsToAvoidRepeating.Intersect(dish.VegetableComponents).Any() &&
                        !dishComponentsToAvoidRepeating.Intersect(dish.FruitComponents).Any();;

                //// Heuristic: Priority components are any components that the user want to see in their plan.
                //// These components may be components that are on-hand or are on sale.
                // Available dishes that contains priority components.
                var highPriorityAvailableDishes = availableDishes
                    .Where(d =>
                        priorityDishComponents.Intersect(d.CarbohydrateComponents).Any() ||
                        priorityDishComponents.Intersect(d.ProteinComponents).Any() ||
                        priorityDishComponents.Intersect(d.VegetableComponents).Any() ||
                        priorityDishComponents.Intersect(d.FruitComponents).Any()
                    ).ToList();

                // Available dishes that does not contain priority components
                var normalPriorityAvailableDishes = availableDishes.Except(highPriorityAvailableDishes).ToList();

                //// Heuristic: Select dishes base on Carbohydrate first, then exclude Carbohydrates from subsequent dishes.
                //// This is required because the list of Carbohydrate components are small, and specifying multiple different types of
                //// Carbohydrate and avoiding repeating component may exhaust all choices.
                Func<IList<Dish>, Dish?> selectDishWithCarbohydrate = (dishes) =>
                {
                    return PickRandomOrDefault(dishes
                        .Where(dish => dish.CarbohydrateComponents.Any())
                        .Where(doesNotContainRepeatingComponents)
                        .ToList());
                };

                var dishWithCarbohydrate = selectDishWithCarbohydrate(highPriorityAvailableDishes) ?? selectDishWithCarbohydrate(normalPriorityAvailableDishes);

                currentDishes.Add(dishWithCarbohydrate);
                dishComponentsToAvoidRepeating.AddRange(GetComponents(dishWithCarbohydrate));

                // Predicate that tests if a dish does not contain carbohydrate components 
                Func<Dish, bool> doesNotContainCarbohydrates = (dish) => !dish.CarbohydrateComponents.Any();

                // Select Dishes with Protein if required
                if (currentDishes.All(d => !d.ProteinComponents.Any()))
                {
                    Func<IList<Dish>, Dish?> selectDishWithProtein = (dishes) =>
                    {
                        return PickRandomOrDefault(dishes
                            .Where(dish => dish.ProteinComponents.Any())
                            .Where(doesNotContainCarbohydrates)
                            .Where(doesNotContainRepeatingComponents)
                            .ToList());
                    };

                    var dishesWithProtein = selectDishWithProtein(highPriorityAvailableDishes) ?? selectDishWithProtein(normalPriorityAvailableDishes);

                    currentDishes.Add(dishesWithProtein);
                    dishComponentsToAvoidRepeating.AddRange(GetComponents(dishesWithProtein));
                }

                // Select Dishes with Vegetable if required
                if (currentDishes.All(d => !d.VegetableComponents.Any()))
                {
                    Func<IList<Dish>, Dish?> selectDishWithVegetable = (dishes) =>
                    {
                        return PickRandomOrDefault(dishes
                            .Where(dish => dish.VegetableComponents.Any())
                            .Where(doesNotContainCarbohydrates)
                            .Where(doesNotContainRepeatingComponents)
                            .ToList());
                    };

                    var dishWithVegetable = selectDishWithVegetable(highPriorityAvailableDishes) ?? selectDishWithVegetable(normalPriorityAvailableDishes);

                    currentDishes.Add(dishWithVegetable);
                    dishComponentsToAvoidRepeating.AddRange(GetComponents(dishWithVegetable));
                }

                // Select Dishes with Vegetable if required
                if (currentDishes.All(d => !d.FruitComponents.Any()))
                {
                    Func<IList<Dish>, Dish?> selectDishWithFruit = (dishes) =>
                    {
                        return PickRandomOrDefault(dishes
                            .Where(dish => dish.FruitComponents.Any())
                            .Where(doesNotContainCarbohydrates)
                            .Where(doesNotContainRepeatingComponents)
                            .ToList());
                    };

                    var dishWithFruit = selectDishWithFruit(highPriorityAvailableDishes) ?? selectDishWithFruit(normalPriorityAvailableDishes);

                    currentDishes.Add(dishWithFruit);
                    dishComponentsToAvoidRepeating.AddRange(GetComponents(dishWithFruit));
                }

                mealPlan.Add(
                    new Meal(
                        DateOnly.FromDateTime(DateTime.Today.AddDays(i)),
                        MealType.DINNER,
                        currentDishes));
            }

            return mealPlan;
        }

        /// <summary>
        /// Randomly select an element from a list, or the default if the list is empty or null
        /// </summary>
        private T? PickRandomOrDefault<T>(IList<T> items)
        {
            if (items != null && items.Count > 0)
            {
                return items[_randomNumberGenerator.Next(0, items.Count - 1)];
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Returns all components of a dish in a single List
        /// </summary>
        private IList<string> GetComponents(Dish dish)
        {
            return
                dish.CarbohydrateComponents.Concat(
                dish.ProteinComponents.Concat(
                dish.VegetableComponents.Concat(
                dish.FruitComponents)))
                .ToList();
        }
    }
}
