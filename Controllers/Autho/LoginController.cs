using Microsoft.AspNetCore.Mvc;

namespace PBL3.Controllers.Autho
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
