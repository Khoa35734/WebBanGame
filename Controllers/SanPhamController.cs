using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanGame.Models;
namespace WebBanGame.Controllers

{
    public class SanPhamController : Controller
    {
        private readonly BanGameBanQuyenContext _context;

        public SanPhamController(BanGameBanQuyenContext context)
        {
            _context = context;
        }

        // GET: SanPham
        public async Task<IActionResult> Index()
        {
            var danhSachGame = _context.SanPhams
         .Include(g => g.DanhMucs)
         .OrderBy(g => g.MaSp)
         .ToList();
            return View(danhSachGame);
        }

        // GET: SanPham/Details/5
        public IActionResult Details(int id)
        {
            var game = _context.SanPhams
                .Include(sp => sp.DanhMucs)
                .FirstOrDefault(sp => sp.MaSp == id);

            if (game == null)
                return NotFound();

            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                // Lấy id user từ claim/session tùy hệ thống của bạn
                // Ví dụ: userId = int.Parse(User.FindFirst("UserId").Value);
                userId = GetCurrentUserId(); // bạn tự viết hàm này theo hệ thống đăng nhập
            }

            bool daMua = false;
            string linkTaiVe = game.LinkDownGame ?? "#";

            if (userId.HasValue)
            {
                daMua = _context.DonHangs
                    .Where(dh => dh.MaKh == userId.Value && dh.ThanhToan)
                    .SelectMany(dh => dh.ChiTietDonHangs)
                    .Any(ct => ct.MaSp == id);
            }

            ViewBag.DaMua = daMua;
            ViewBag.LinkTaiVe = linkTaiVe;

            return View(game);
        }
        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
        // GET: SanPham/Create
        public IActionResult Create()
        {
            ViewData["MaDm"] = new SelectList(_context.DanhMucSps, "MaDm", "TenDm");
            return View();
        }

        // POST: SanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,MaDm,TenSp,AnhSp,VideoSp,GiaSp,TrangThai,BestSeller,CreateDate,NgaySua,MotaSp,LinkDownGame")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDm"] = new SelectList(_context.DanhMucSps, "MaDm", "TenDm", sanPham.DanhMucs);
            return View(sanPham);
        }
        // Controller
        public IActionResult Search(string query)
        {
            var games = _context.SanPhams
                .Include(g => g.DanhMucs)
                .Where(g => g.TenSp.Contains(query))
                .ToList();

            var model = new SearchViewModel
            {
                Query = query,
                Games = games
            };
            return View(model);
        }
        public async Task<ActionResult> BestSellerList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMucs)
                .FirstOrDefaultAsync(m => m.MaSp == id && m.BestSeller == true);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }
     
        public ActionResult SearchDm(int danhMucId)
        {
            // Lấy danh sách các game thuộc danh mục
            var danhMuc = _context.DanhMucSps.FirstOrDefault(dm => dm.MaDm == danhMucId);
           

            var games = _context.SanPhams
                .Where(sp => sp.DanhMucs.Any(dm => dm.MaDm == danhMucId))
                .ToList();

            // Tạo ViewModel để truyền dữ liệu đến View
            var viewModel = new SearchDmViewModel
            {
                DanhMuc = danhMuc,  // hoặc property phù hợp
                Games = games
            };

            return View(viewModel);
        }
    
        // GET: SanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            var selectedMaDm = sanPham.DanhMucs?.Select(dm => dm.MaDm) ?? new List<int>();
            ViewData["MaDm"] = new MultiSelectList(_context.DanhMucSps, "MaDm", "TenDm", selectedMaDm);
            return View(sanPham);
        }

        // POST: SanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSp,MaDm,TenSp,AnhSp,VideoSp,GiaSp,TrangThai,BestSeller,CreateDate,NgaySua,MotaSp,LinkDownGame")] SanPham sanPham)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var selectedMaDm = sanPham.DanhMucs?.Select(dm => dm.MaDm) ?? new List<int>();
            ViewData["MaDm"] = new MultiSelectList(_context.DanhMucSps, "MaDm", "TenDm", selectedMaDm);
            return View(sanPham);
        }

        // GET: SanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMucs)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public class MuaGameRequest { public int GameId { get; set; } }
        [HttpPost]

        [ValidateAntiForgeryToken]
     
        public JsonResult MuaGame([FromBody] MuaGameRequest req)
        {
            try
            {
                // Lấy thông tin game
                var game = _context.SanPhams.FirstOrDefault(g => g.MaSp == req.GameId);
                if (game == null)
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });

                // Lấy thông tin người dùng
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var customer = _context.KhachHangs.FirstOrDefault(c => c.MaKh.ToString() == userId);
                if (customer == null)
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện giao dịch." });

                // Kiểm tra số dư tài khoản
                if (customer.SoDuTk < game.GiaSp)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Số dư TK không đủ để mua game này.Vui lòng nạp thêm tiền!"
                    });
                }

                // Trừ số dư tài khoản
                customer.SoDuTk -= game.GiaSp;

                // Tạo hóa đơn mua game
                var order = new DonHang
                {
                    MaKh = customer.MaKh,
                    NgayTao = DateOnly.FromDateTime(DateTime.Now),
                    TrangThaiHuyDon = false,
                    ThanhToan = true,
                    NgayThanhToan = DateOnly.FromDateTime(DateTime.Now),
                    Note = "Mua game thành công!"
                };
                _context.DonHangs.Add(order);
                _context.SaveChanges();

                // Thêm chi tiết hóa đơn
                var orderDetail = new ChiTietDonHang
                {
                    MaDh = order.MaDh,
                    MaSp = game.MaSp,
                    TongTien = game.GiaSp,
                    Ngaygiao = 0 // nếu giao ngay
                };
                _context.ChiTietDonHangs.Add(orderDetail);

                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Bạn đã mua game thành công!",
                    downloadLink = game.LinkDownGame
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
    }
}
