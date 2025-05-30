using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanGame.Models;
using Microsoft.AspNetCore.Http;

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
            ViewBag.AllDanhMucs = _context.DanhMucSps
                .Select(dm => new SelectListItem
                {
                    Value = dm.MaDm.ToString(),
                    Text = dm.TenDm
                }).ToList();
            return View();
        }

        // POST: Admin/AdminProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamEditViewModel model, IFormFile AnhUpload)
        {
            // Validate: Bắt buộc chọn ít nhất 1 danh mục
            if (model.SelectedDanhMucs == null || !model.SelectedDanhMucs.Any())
            {
                ModelState.AddModelError("SelectedDanhMucs", "Bạn phải chọn ít nhất một danh mục.");
            }

            if (!ModelState.IsValid)
            {
                model.AllDanhMucs = _context.DanhMucSps
                    .Select(dm => new SelectListItem
                    {
                        Value = dm.MaDm.ToString(),
                        Text = dm.TenDm
                    }).ToList();
                return View(model);
            }

            var sanPham = new SanPham
            {
                TenSp = model.TenSp,
                GiaSp = model.GiaSp,
                TrangThai = model.TrangThai,
                BestSeller = model.BestSeller,
                CreateDate = model.CreateDate,
                NgaySua = DateOnly.FromDateTime(DateTime.Now),
                LinkDownGame = model.LinkDownGame,
                MotaSp = model.MotaSp,
                DanhMucs = new List<DanhMucSp>()
            };

            // Xử lý upload ảnh
            if (AnhUpload != null && AnhUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "games");
                Directory.CreateDirectory(uploadsFolder);
                var ext = Path.GetExtension(AnhUpload.FileName);
                var fileName = Path.GetFileNameWithoutExtension(AnhUpload.FileName)
                            + "_" + Guid.NewGuid().ToString("N") + ext;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhUpload.CopyToAsync(stream);
                }
                sanPham.AnhSp = "/images/games/" + fileName;
            }
            else if (!string.IsNullOrEmpty(model.AnhSp))
            {
                sanPham.AnhSp = model.AnhSp;
            }

            // Xử lý danh mục n-n
            var danhMucsMoi = await _context.DanhMucSps
                .Where(dm => model.SelectedDanhMucs.Contains(dm.MaDm))
                .ToListAsync();
            foreach (var dm in danhMucsMoi)
            {
                sanPham.DanhMucs.Add(dm);
            }

            _context.Add(sanPham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPhamEditViewModel model, IFormFile? AnhUpload)
        {
            if (id != model.MaSp)
            {
                return NotFound();
            }

            // Validate: Bắt buộc chọn ít nhất 1 danh mục
            if (model.SelectedDanhMucs == null || !model.SelectedDanhMucs.Any())
            {
                ModelState.AddModelError("SelectedDanhMucs", "Bạn phải chọn ít nhất một danh mục.");
            }

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

            var sanPham = await _context.SanPhams
                .Include(sp => sp.DanhMucs)
                .FirstOrDefaultAsync(sp => sp.MaSp == id);
            if (sanPham == null) return NotFound();

            sanPham.TenSp = model.TenSp;
            sanPham.GiaSp = model.GiaSp;
            sanPham.TrangThai = model.TrangThai;
            sanPham.BestSeller = model.BestSeller;
            sanPham.CreateDate = model.CreateDate;
            sanPham.NgaySua = DateOnly.FromDateTime(DateTime.Now);
            sanPham.LinkDownGame = model.LinkDownGame;
            sanPham.MotaSp = model.MotaSp;

            // Xử lý ảnh (upload mới hoặc giữ cũ)
            if (AnhUpload != null && AnhUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "games");
                Directory.CreateDirectory(uploadsFolder);
                var ext = Path.GetExtension(AnhUpload.FileName);
                var fileName = Path.GetFileNameWithoutExtension(AnhUpload.FileName)
                            + "_" + Guid.NewGuid().ToString("N") + ext;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AnhUpload.CopyToAsync(stream);
                }
                sanPham.AnhSp = "/images/games/" + fileName;
            }
            else if (!string.IsNullOrEmpty(model.AnhSp))
            {
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
            var sanPham = await _context.SanPhams
                .Include(sp => sp.DanhMucs)
                .FirstOrDefaultAsync(sp => sp.MaSp == id);
            if (sanPham != null)
            {
                sanPham.DanhMucs.Clear();
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
    }
}