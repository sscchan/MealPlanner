using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishesController : ControllerBase
    {
        private IDishDataManagementService _dishDataManagementService;

        public DishesController(IDishDataManagementService dishDataManagementService)
        {
            _dishDataManagementService = dishDataManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return new JsonResult(await _dishDataManagementService.GetDishes().ToListAsync());
        }

        [HttpGet]
        [Route("Components")]
        public async Task<IActionResult> GetDishesComponents()
        {
            return new JsonResult(await _dishDataManagementService.GetDishComponents());
        }
    }
}
