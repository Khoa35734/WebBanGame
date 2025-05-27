using Microsoft.AspNetCore.Mvc;
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
            int year = DateTime.Now.Year;
            string[] months = new string[12];
            int[] visits = new int[12]; // Nếu không có bảng logs truy cập, để số mẫu hoặc tự xử lý
            int[] orders = new int[12]; // Số lượt mua hàng/tháng (có thể lấy từ bảng DonHang)
            decimal[] revenues = new decimal[12]; // Tổng số tiền nạp thành công/tháng

            for (int i = 0; i < 12; i++)
            {
                months[i] = $"Tháng {i + 1}";

                // Số lượt truy cập (giả lập, bạn thay bằng truy vấn thật nếu có logs)
                visits[i] = 100 + 15 * i;

                // Số lượt mua hàng/tháng (giả sử lấy theo đơn hàng thanh toán thành công)
                orders[i] = _context.DonHangs
                    .Where(dh => dh.NgayTao.Month == i + 1 && dh.NgayTao.Year == year && dh.ThanhToan)
                    .Count();

                // Tổng doanh thu là tổng số tiền khách hàng nạp thành công theo tháng
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
