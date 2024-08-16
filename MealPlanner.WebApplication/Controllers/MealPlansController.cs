using MealPlanner.Application.Repositories;
using MealPlanner.Application.Services;
using MealPlanner.WebApplication.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.WebApplication.Controllers
{
    [ApiController]
    [Route("api/MealPlans")]
    public class MealPlansController : ControllerBase
    {
        private IMealPlannerService _mealPlannerService;

        public MealPlansController(IMealPlannerService mealPlannerService)
        {
            _mealPlannerService = mealPlannerService;
        }

        /// <summary>
        /// Create a meal plan.
        /// </summary>
        /// <returns>A new meal plan</returns>
        [HttpPost]
        [Route("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IList<PostMealPlansResponseDto>), 200)]
        public async Task<IActionResult> PostMealPlans([FromBody] PostMealPlansRequestDto request)
        {
            var mealPlan = await _mealPlannerService.GenerateMealPlan(
                    numberOfDaysBeforeDishComponentsCanBeUsedAgain: request.NumberOfDaysBeforeDishComponentsCanBeUsedAgain,
                    numberOfDaysInPlan: request.NumberOfDaysInPlan,
                    priorityDishComponents: request.PriorityDishComponents,
                    prohibitedDishComponents: request.ProhibitedDishComponents,
                    prohibitedDishes: request.ProhibitedDishes
                );

            var getMealPlansResponse = mealPlan.Meals.Select(m => new PostMealPlansResponseDto(
                date: m.Date,
                dishNames: string.Join(" | ", m.Dishes.Select(d => d.Name)),
                components: 
                    string.Join(", ", m.Dishes.SelectMany(d => d.CarbohydrateComponents)) + " | " +
                    string.Join(", ", m.Dishes.SelectMany(d => d.ProteinComponents)) + " | " +
                    string.Join(", ", m.Dishes.SelectMany(d => d.VegetableComponents)) + " | " +
                    string.Join(", ", m.Dishes.SelectMany(d => d.FruitComponents))
                ));

            return new JsonResult(getMealPlansResponse);
        }
    }
}
