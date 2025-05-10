using Microsoft.AspNetCore.Mvc;

namespace PBL3.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
