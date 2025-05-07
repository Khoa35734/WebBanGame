using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGame.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Antiforgery;

namespace WEB_GAME_STORE.Controllers
{
    public class OrderController : Controller
    {
        private readonly BanGameBanQuyenContext _context;
        private readonly IAntiforgery _antiforgery;

        public OrderController(BanGameBanQuyenContext context,IAntiforgery antiforgery)
        {
            _context = context;
            _antiforgery = antiforgery;
        }
        public IActionResult Details(int id)
        {
            var chiTiet = _context.ChiTietDonHangs
                .Include(ct => ct.MaSpNavigation)
                .Include(ct => ct.MaDhNavigation)
                .Where(ct => ct.MaDh == id)
                .ToList();

            return View(chiTiet);
        }

        [HttpPost]
        public IActionResult Create([FromBody] int[] idGames)
        {
            if (idGames == null || idGames.Length == 0)
                return BadRequest("Danh sách game không hợp lệ.");

            double tongTien = 0;
            var danhSachGame = _context.SanPhams
                .Where(g => idGames.Contains(g.MaSp))
                .ToList();

            foreach (var game in danhSachGame)
            {
                tongTien += (double)(game.GiaSp);
            }

            // TODO: Lấy ID khách hàng từ User.Identity sau khi đăng nhập
            int maKhachHang = 1;

            var donHang = new DonHang
            {
                MaKh = maKhachHang,
                NgayTao = DateTime.Now,
                TrangThaiHuyDon = false,
                ThanhToan = false,
                NgayThanhToan = default, // có thể cập nhật sau khi thanh toán
                Note = null
            };

            _context.DonHangs.Add(donHang);
            _context.SaveChanges(); // để có MaDh cho khóa ngoại

            foreach (var game in danhSachGame)
            {
                var chiTiet = new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaSp = game.MaSp,
                    TongTien = Convert.ToInt32(game.GiaSp),
                    Ngaygiao = int.Parse(DateTime.Now.ToString("yyyyMMdd")) // lưu ngày dạng số (YYYYMMDD)
                };

                _context.ChiTietDonHangs.Add(chiTiet);
            }

            _context.SaveChanges();

            return Ok(new { message = "Mua game thành công", donHang.MaDh });
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Purchase([FromBody] int[] gameIds)
		{
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
			{
				return Json(new { success = false, message = "Bạn cần đăng nhập để mua hàng." });
			}

			var customer = _context.KhachHangs.FirstOrDefault(k => k.MaKh.ToString() == userId);
			if (customer == null)
			{
				return Json(new { success = false, message = "Người dùng không tồn tại." });
			}

			var selectedGames = _context.SanPhams.Where(sp => gameIds.Contains(sp.MaSp)).ToList();
			if (!selectedGames.Any())
			{
				return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
			}

			var totalCost = selectedGames.Sum(g => g.GiaSp);
			if (customer.SoDuTk < totalCost)
			{
				return Json(new { success = false, message = "Số dư không đủ để thanh toán." });
			}

			// Create DonHang (Order)
			var newOrder = new DonHang
			{
				MaKh = customer.MaKh,
				NgayTao = DateTime.Now.Date,
				TrangThaiHuyDon = false,
				ThanhToan = true,
				NgayThanhToan = DateTime.Now.Date
			};
			_context.DonHangs.Add(newOrder);
			_context.SaveChanges();

			// Add ChiTietDonHang (Order Details)
			foreach (var game in selectedGames)
			{
				var orderDetail = new ChiTietDonHang
				{
					MaDh = newOrder.MaDh,
					MaSp = game.MaSp,
					TongTien = game.GiaSp,
					Ngaygiao = 3 // Example: Delivery in 3 days
				};
				_context.ChiTietDonHangs.Add(orderDetail);
			}

			// Deduct from customer balance
			customer.SoDuTk -= totalCost;

			_context.SaveChanges();

			// Provide download link
			var downloadLinks = selectedGames.Select(g => g.LinkDownGame).ToList();
			return Json(new { success = true, message = "Mua hàng thành công!", downloadLinks });
		}

	}
}

