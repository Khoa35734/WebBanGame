
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBanGame.Models;
namespace WebBanGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly BanGameBanQuyenContext _context;

        public HomeController(BanGameBanQuyenContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var danhSachGame = _context.SanPhams
    .Include(g => g.DanhMucs)
    .OrderBy(g => g.MaSp).Take(6)
    .ToList();
            return View(danhSachGame);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
