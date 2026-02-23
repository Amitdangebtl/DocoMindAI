using Microsoft.AspNetCore.Mvc;

namespace MVCSimpleUplode.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
