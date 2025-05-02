using Microsoft.AspNetCore.Mvc;

namespace PBL3.Controllers.Autho
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult QLSV()
        {
            return View();
        }
    }
}
