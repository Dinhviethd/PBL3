using Microsoft.AspNetCore.Mvc;

namespace PBL3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
