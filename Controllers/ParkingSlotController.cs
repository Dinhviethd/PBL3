using Microsoft.AspNetCore.Mvc;

namespace PBL3.Controllers
{
    public class ParkingSlotController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
