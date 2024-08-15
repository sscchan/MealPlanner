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

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IList<GetMealPlansResponseDto>), 200)]
        public async Task<IActionResult> GetMealPlans([FromQuery] string[] avoidComponents)
        {
            var mealPlan = await _mealPlannerService.GenerateNewMealPlan(initialComponentsToAvoidDuplicates: avoidComponents.ToList());

            var getMealPlansResponse = mealPlan.Meals.Select(m => new GetMealPlansResponseDto(
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
