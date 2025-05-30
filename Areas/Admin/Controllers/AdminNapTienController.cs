using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGame.Models;
using System.Linq;

namespace WebBanGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminNapTienController : Controller
    {
        private readonly BanGameBanQuyenContext _context;

        public AdminNapTienController(BanGameBanQuyenContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminNapTien
        public async Task<IActionResult> Index(string searchTen)
        {
            var napTiens = _context.NapTiens
                .Include(n => n.IdUserNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTen))
            {
                napTiens = napTiens.Where(n => n.IdUserNavigation != null
                    && n.IdUserNavigation.TenKh.Contains(searchTen));
            }

            var model = await napTiens
                .OrderByDescending(n => n.NgayNap)
                .ToListAsync();

            ViewBag.SearchTen = searchTen;
            return View(model);
        }
    }
}