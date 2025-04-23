using Microsoft.AspNetCore.Mvc;

namespace WebBanGame.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult DangNhap()
		{
			return View();
		}
        public IActionResult DangKy()
        {
            return View();
        }
    }
}
