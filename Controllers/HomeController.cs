using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebBanGame.Models;
using System.Collections.Generic;

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
            // Lấy số lượt bán cho mỗi sản phẩm
            var soLanBanDict = _context.ChiTietDonHangs
                .GroupBy(ct => ct.MaSp)
                .Select(g => new { MaSp = g.Key, SoLanBan = g.Count() })
                .ToDictionary(x => x.MaSp, x => x.SoLanBan);

            // Lấy tất cả sản phẩm, kèm danh mục
            var allGames = _context.SanPhams
                .Include(sp => sp.DanhMucs)
                .ToList();

            // Tạo ViewModel chứa thông tin game và số lần bán
            var vmList = allGames
                .Select(g => new SanPhamWithSoLanBanViewModel
                {
                    MaSp = g.MaSp,
                    TenSp = g.TenSp,
                    AnhSp = g.AnhSp,
                    MotaSp = g.MotaSp,
                    GiaSp = g.GiaSp,
                    // Nếu NgaySua là DateOnly
                    NgaySua = g.NgaySua.ToDateTime(TimeOnly.MinValue),
                    DanhMucs = g.DanhMucs,
                    SoLanBan = soLanBanDict.ContainsKey(g.MaSp) ? soLanBanDict[g.MaSp] : 0
                })
                .ToList();

            return View(vmList);
        }
    }

    // ViewModel cho View
    public class SanPhamWithSoLanBanViewModel
    {
        public int MaSp { get; set; }
        public string TenSp { get; set; }
        public string AnhSp { get; set; }
        public string MotaSp { get; set; }
        public decimal GiaSp { get; set; }
        public DateTime NgaySua { get; set; }
        public ICollection<DanhMucSp> DanhMucs { get; set; }
        public int SoLanBan { get; set; }
    }
}