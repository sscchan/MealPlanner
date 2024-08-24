using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.WebApplication.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("./swagger");
        }
    }
}
