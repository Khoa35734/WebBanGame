using Microsoft.AspNetCore.Mvc;

namespace WebBanGame.Areas.Admin.Controllers
{
    public class AdminOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
