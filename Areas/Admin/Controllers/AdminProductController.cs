using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanGame.Models;

namespace WebBanGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductController : Controller
    {
        private readonly BanGameBanQuyenContext _context;

        public AdminProductController(BanGameBanQuyenContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminProduct
        public async Task<IActionResult> Index()
        {
            var banGameBanQuyenContext = _context.SanPhams.Include(s => s.MaDmNavigation);
            return View(await banGameBanQuyenContext.ToListAsync());
        }

        // GET: Admin/AdminProduct/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDmNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Admin/AdminProduct/Create
        public IActionResult Create()
        {
            ViewData["MaDm"] = new SelectList(_context.DanhMucSps, "MaDm", "TenDm");
            return View();
        }

        // POST: Admin/AdminProduct/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(SanPham sanPham, IFormFile AnhUpload)
        {
            // Xử lý upload ảnh trước khi kiểm tra ModelState.IsValid
            if (AnhUpload != null && AnhUpload.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(AnhUpload.FileName)
                            + "_" + Guid.NewGuid().ToString("N")
                            + Path.GetExtension(AnhUpload.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/games", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhUpload.CopyToAsync(stream);
                }
                // Gán đường dẫn vào thuộc tính AnhSp
                sanPham.AnhSp = "/images/games/" + fileName;
            }

            // Sau khi đã gán AnhSp, mới kiểm tra ModelState
            if (ModelState.IsValid)
            {
                // Các xử lý khác...
                sanPham.CreateDate = DateOnly.FromDateTime(DateTime.Now);
                sanPham.NgaySua = DateOnly.FromDateTime(DateTime.Now);

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi, trả về view và model để hiển thị lỗi
            ViewBag.MaDm = new SelectList(_context.DanhMucSps, "MaDm", "TenDm", sanPham.MaDm);
            return View(sanPham);
        }

        // GET: Admin/AdminProduct/Edit/5
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
            ViewData["MaDm"] = new SelectList(_context.DanhMucSps, "MaDm", "TenDm", sanPham.MaDm);
            return View(sanPham);
        }

        // POST: Admin/AdminProduct/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
     int id,
     [Bind("MaSp,MaDm,TenSp,AnhSp,VideoSp,GiaSp,TrangThai,BestSeller,CreateDate,NgaySua,MotaSp,LinkDownGame")] SanPham sanPham,
     IFormFile? AnhUpload)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            // Lấy entity gốc từ DB
            var sanPhamGoc = await _context.SanPhams.FindAsync(id);
            if (sanPhamGoc == null)
            {
                return NotFound();
            }

            // Nếu không upload ảnh mới và AnhSp bị rỗng, gán lại path ảnh cũ
            if ((AnhUpload == null || AnhUpload.Length == 0) && string.IsNullOrEmpty(sanPham.AnhSp))
            {
                sanPham.AnhSp = sanPhamGoc.AnhSp;
            }

            // Xử lý upload file ảnh nếu có upload mới
            if (AnhUpload != null && AnhUpload.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(AnhUpload.FileName)
                             + "_" + Guid.NewGuid().ToString("N")
                             + Path.GetExtension(AnhUpload.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/games", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhUpload.CopyToAsync(stream);
                }
                sanPhamGoc.AnhSp = "/images/games/" + fileName;
            }
            else
            {
                // Không upload mới, giữ nguyên giá trị từ input hidden
                sanPhamGoc.AnhSp = sanPham.AnhSp;
            }

            // Cập nhật các trường khác
            sanPhamGoc.MaDm = sanPham.MaDm;
            sanPhamGoc.TenSp = sanPham.TenSp;
          
            sanPhamGoc.GiaSp = sanPham.GiaSp;
            sanPhamGoc.TrangThai = sanPham.TrangThai;
            sanPhamGoc.BestSeller = sanPham.BestSeller;
            sanPhamGoc.CreateDate = sanPham.CreateDate;
            sanPhamGoc.NgaySua = DateOnly.FromDateTime(DateTime.Now);
            sanPhamGoc.MotaSp = sanPham.MotaSp;
            sanPhamGoc.LinkDownGame = sanPham.LinkDownGame;

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
                        return NotFound();
                    else
                        throw;
                }
            }
            else
            {
                // Debug lỗi ModelState nếu có
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"ModelState Error - Field: {key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            ViewData["MaDm"] = new SelectList(_context.DanhMucSps, "MaDm", "TenDm", sanPham.MaDm);
            // Trả về lại model vừa nhập (có giá trị AnhSp) để giữ đúng hidden input khi render lại view
            return View(sanPham);
        }
        // GET: Admin/AdminProduct/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDmNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Admin/AdminProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
    }
}
