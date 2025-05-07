using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_GAME_STORE.Models;
using System;
using System.Linq;

namespace WEB_GAME_STORE.Controllers
{
    public class OrderController : Controller
    {
        private readonly Pbl3Context _context;

        public OrderController(Pbl3Context context)
        {
            _context = context;
        }

        // GET: /Order
        public IActionResult Index()
        {
            var orders = _context.HoaDons
                .Include(o => o.IdUserNavigation)
                .ToList();

            return View(orders);
        }

        // GET: /Order/Details/5
        public IActionResult Details(int id)
        {
            var chiTiet = _context.ChiTietHoaDons
                .Include(ct => ct.IdGameNavigation)
                .Include(ct => ct.IdHoaDonNavigation)
                .Where(ct => ct.IdHoaDon == id)
                .ToList();

            return View(chiTiet);
        }

        // POST: /Order/Create
        [HttpPost]
        public IActionResult Create([FromBody] int[] idGames)
        {
            if (idGames == null || idGames.Length == 0)
                return BadRequest("Danh sách game không hợp lệ.");

            double tongTien = 0;
            var danhSachGame = _context.TbGames
                .Where(g => idGames.Contains(g.IdGame))
                .ToList();

            foreach (var game in danhSachGame)
            {
                tongTien += (double)(game.GiaBanSale != 0 ? game.GiaBanSale : game.GiaBan ?? 0);
            }

            var hoaDon = new HoaDon
            {
                IdUser = 1,  //Đăng nhập vào thì dùng id user
                ThoiGianLap = DateTime.Now,
                GiaTien = tongTien,
                XuLy = 0
            };

            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();

            foreach (var game in danhSachGame)
            {
                var chiTiet = new ChiTietHoaDon
                {
                    IdHoaDon = hoaDon.IdHoadon,
                    IdGame = game.IdGame,
                    TenGame = game.TenGame,
                    GiaBan = game.GiaBanSale != 0 ? game.GiaBanSale : (game.GiaBan ?? 0)
                };
                _context.ChiTietHoaDons.Add(chiTiet);
            }

            _context.SaveChanges();

            return Ok(new { message = "Đặt hàng thành công", hoaDon.IdHoadon });
        }
    }
}

