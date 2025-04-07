using Microsoft.AspNetCore.Mvc;

namespace APDP_ASM2.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
