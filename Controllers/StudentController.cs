using Microsoft.AspNetCore.Mvc;

namespace PBL3.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
