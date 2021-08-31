using Microsoft.AspNetCore.Mvc;

namespace Module1.Controllers
{
    [Route("Module1/{controller}/{action}")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NotExist()
        {
            return View();
        }
    }
}
