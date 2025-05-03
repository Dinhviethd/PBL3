using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PBL3.Controllers.Autho
{
    // Apply authorization to the entire controller

   // [Authorize(Roles = "Admin")] // Restrict access to users in the "Admin" role
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
