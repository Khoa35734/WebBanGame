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
            var banGameBanQuyenContext = _context.SanPhams.Include(s => s.DanhMucs);
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
                .Include(s => s.DanhMucs)
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
            ViewBag.MaDm = _context.DanhMucSps.ToList();
            ViewBag.SelectedDanhMucIds = new List<int>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham, IFormFile AnhUpload, int[] SelectedDanhMucIds)
        {
            // Validate danh mục
            if (SelectedDanhMucIds == null || SelectedDanhMucIds.Length == 0)
                ModelState.AddModelError("SelectedDanhMucIds", "Bạn phải chọn ít nhất 1 danh mục.");

            // Xử lý upload ảnh và gán AnhSp TRƯỚC validate ModelState
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
                sanPham.AnhSp = "/images/games/" + fileName;
            }

            if (string.IsNullOrEmpty(sanPham.AnhSp))
                ModelState.AddModelError("AnhSp", "Bạn phải chọn ảnh game!");


            if (ModelState.IsValid)
            {
                sanPham.CreateDate = DateOnly.FromDateTime(DateTime.Now);
                sanPham.NgaySua = DateOnly.FromDateTime(DateTime.Now);
                sanPham.DanhMucs = _context.DanhMucSps
                    .Where(dm => SelectedDanhMucIds.Contains(dm.MaDm)).ToList();

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nạp lại ViewBag nếu có lỗi
            ViewBag.MaDm = _context.DanhMucSps.ToList();
            ViewBag.SelectedDanhMucIds = SelectedDanhMucIds?.ToList() ?? new List<int>();
            return View(sanPham);
        }

        // GET: Admin/AdminProduct/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(sp => sp.DanhMucs)
                .FirstOrDefaultAsync(sp => sp.MaSp == id);
            if (sanPham == null) return NotFound();

            var allDanhMucs = await _context.DanhMucSps
                .Select(dm => new SelectListItem
                {
                    Value = dm.MaDm.ToString(),
                    Text = dm.TenDm
                }).ToListAsync();

            var model = new SanPhamEditViewModel
            {
                MaSp = sanPham.MaSp,
                TenSp = sanPham.TenSp,
                AnhSp = sanPham.AnhSp,
                GiaSp = sanPham.GiaSp,
                TrangThai = sanPham.TrangThai,
                BestSeller = sanPham.BestSeller,
                CreateDate = sanPham.CreateDate,
                NgaySua = sanPham.NgaySua,
                LinkDownGame = sanPham.LinkDownGame,
                MotaSp = sanPham.MotaSp,
                SelectedDanhMucs = sanPham.DanhMucs.Select(dm => dm.MaDm).ToList(),
                AllDanhMucs = allDanhMucs
            };

            return View(model);
        }


        // POST: Admin/AdminProduct/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SanPhamEditViewModel model, IFormFile? AnhUpload)
        {
            // Validate: Bắt buộc chọn ít nhất 1 danh mục
            if (model.SelectedDanhMucs == null || !model.SelectedDanhMucs.Any())
            {
                ModelState.AddModelError("SelectedDanhMucs", "Bạn phải chọn ít nhất một danh mục.");
            }

            // Nếu có lỗi, nạp lại danh mục cho ViewModel rồi trả lại view
            if (!ModelState.IsValid)
            {
                model.AllDanhMucs = await _context.DanhMucSps
                    .Select(dm => new SelectListItem
                    {
                        Value = dm.MaDm.ToString(),
                        Text = dm.TenDm
                    }).ToListAsync();

                return View(model);
            }

            // Lấy sản phẩm hiện tại
            var sanPham = await _context.SanPhams
                .Include(sp => sp.DanhMucs)
                .FirstOrDefaultAsync(sp => sp.MaSp == model.MaSp);

            if (sanPham == null) return NotFound();

            // Cập nhật thông tin cơ bản
            sanPham.TenSp = model.TenSp;
            sanPham.GiaSp = model.GiaSp;
            sanPham.TrangThai = model.TrangThai;
            sanPham.BestSeller = model.BestSeller;
            sanPham.CreateDate = model.CreateDate;
            sanPham.NgaySua = DateOnly.FromDateTime(DateTime.Now);
            sanPham.LinkDownGame = model.LinkDownGame;
            sanPham.MotaSp = model.MotaSp;

            // Xử lý ảnh (nếu có upload mới)
            if (AnhUpload != null && AnhUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "games");
                Directory.CreateDirectory(uploadsFolder); // đảm bảo folder tồn tại

                var ext = Path.GetExtension(AnhUpload.FileName);
                var fileName = Path.GetFileNameWithoutExtension(AnhUpload.FileName)
                             + "_" + Guid.NewGuid().ToString("N")
                             + ext;
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhUpload.CopyToAsync(stream);
                }

                // Nếu muốn: Xóa file cũ trên máy chủ (option)
                // if (!string.IsNullOrEmpty(sanPham.AnhSp))
                // {
                //     var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", sanPham.AnhSp.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                //     if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                // }

                sanPham.AnhSp = "/images/games/" + fileName;
            }
            else if (!string.IsNullOrEmpty(model.AnhSp))
            {
                // Không upload mới => giữ ảnh cũ
                sanPham.AnhSp = model.AnhSp;
            }
            // Nếu không upload và model.AnhSp rỗng => giữ nguyên sanPham.AnhSp hiện tại (không thay đổi)

            // Xử lý quan hệ n-n: cập nhật danh mục
            sanPham.DanhMucs.Clear();
            if (model.SelectedDanhMucs != null && model.SelectedDanhMucs.Any())
            {
                var danhMucsMoi = await _context.DanhMucSps
                    .Where(dm => model.SelectedDanhMucs.Contains(dm.MaDm))
                    .ToListAsync();
                foreach (var dm in danhMucsMoi)
                {
                    sanPham.DanhMucs.Add(dm);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminProduct/Delete/5
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
