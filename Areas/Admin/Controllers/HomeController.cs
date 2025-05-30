using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using WebBanGame.Models;

namespace WebBanGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly BanGameBanQuyenContext _context;

        public HomeController(BanGameBanQuyenContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy top 5 game bán chạy nhất (dựa trên số lượt xuất hiện trong ChiTietDonHang)
            var topGames = _context.ChiTietDonHangs
                .Include(ct => ct.MaSpNavigation)
                .GroupBy(ct => new { ct.MaSp, ct.MaSpNavigation.TenSp, ct.MaSpNavigation.AnhSp })
                .Select(g => new
                {
                    TenSp = g.Key.TenSp,
                    AnhSp = g.Key.AnhSp,
                    TongHoaDon = g.Count()
                })
                .OrderByDescending(x => x.TongHoaDon)
                .Take(5)
                .ToList();
            ViewBag.TopGames = topGames;

            int year = DateTime.Now.Year;
            string[] months = new string[12];
            int[] visits = new int[12]; // Dữ liệu demo, bạn thay thế bằng logs thực tế nếu có
            int[] orders = new int[12];
            decimal[] revenues = new decimal[12];

            for (int i = 0; i < 12; i++)
            {
                months[i] = $"Tháng {i + 1}";

                // Số lượt truy cập (giả lập, thay bằng truy vấn logs nếu có)
                visits[i] = 100 + 15 * i;

                // Số lượt mua hàng/tháng (dựa vào DonHang đã thanh toán)
                orders[i] = _context.DonHangs
                    .Where(dh => dh.NgayTao.Month == i + 1 && dh.NgayTao.Year == year && dh.ThanhToan)
                    .Count();

                // Tổng doanh thu/tháng (tổng số tiền nạp thành công)
                revenues[i] = _context.NapTiens
                    .Where(n => n.NgayNap.HasValue
                                && n.NgayNap.Value.Month == i + 1
                                && n.NgayNap.Value.Year == year
                                && n.TrangThai == "thanhcong"
                                && n.SoTienNap.HasValue)
                    .Sum(n => (decimal?)n.SoTienNap) ?? 0;
            }

            // Tổng doanh thu toàn năm
            decimal tongDoanhThu = _context.NapTiens
                .Where(n => n.NgayNap.HasValue
                            && n.NgayNap.Value.Year == year
                            && n.TrangThai == "thanhcong"
                            && n.SoTienNap.HasValue)
                .Sum(n => (decimal?)n.SoTienNap) ?? 0;

            ViewBag.Months = JsonConvert.SerializeObject(months);
            ViewBag.Visits = JsonConvert.SerializeObject(visits);
            ViewBag.Orders = JsonConvert.SerializeObject(orders);
            ViewBag.Revenues = JsonConvert.SerializeObject(revenues);
            ViewBag.TongDoanhThu = tongDoanhThu;

            return View();
        }
    }
}