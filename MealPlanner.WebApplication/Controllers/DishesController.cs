using MealPlanner.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishesController : ControllerBase
    {
        private IDishRepository _dishRepository;

        public DishesController(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return new JsonResult(await _dishRepository.GetAll().ToListAsync());
        }             
    }
}
