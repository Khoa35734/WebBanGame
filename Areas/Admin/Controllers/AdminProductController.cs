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
        public async Task<IActionResult> Create([Bind("MaSp,MaDm,TenSp,AnhSp,VideoSp,GiaSp,TrangThai,BestSeller,CreateDate,NgaySua,MotaSp,LinkDownGame")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDm"] = new SelectList(_context.DanhMucSps, "MaDm", "TenDm", sanPham.MaDm);
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
        public async Task<IActionResult> Edit(int id, [Bind("MaSp,MaDm,TenSp,AnhSp,VideoSp,GiaSp,TrangThai,BestSeller,CreateDate,NgaySua,MotaSp,LinkDownGame")] SanPham sanPham)
        {
            // Validate: Bắt buộc chọn ít nhất 1 danh mục
            if (model.SelectedDanhMucs == null || !model.SelectedDanhMucs.Any())
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
