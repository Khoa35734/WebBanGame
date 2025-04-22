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
    public class AccountController : Controller
    {
        private readonly BanGameBanQuyenContext _context;

        public AccountController(BanGameBanQuyenContext context)
        {
            _context = context;
        }

        // GET: Admin/Account
        public async Task<IActionResult> Index()
        {
            return View(await _context.AccountAdmins.ToListAsync());
        }

        // GET: Admin/Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountAdmin = await _context.AccountAdmins
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (accountAdmin == null)
            {
                return NotFound();
            }

            return View(accountAdmin);
        }

        // GET: Admin/Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,TaiKhoan,MatKhau,Phone,Email,FullName,CreateDate")] AccountAdmin accountAdmin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accountAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountAdmin);
        }

        // GET: Admin/Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountAdmin = await _context.AccountAdmins.FindAsync(id);
            if (accountAdmin == null)
            {
                return NotFound();
            }
            return View(accountAdmin);
        }

        // POST: Admin/Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,TaiKhoan,MatKhau,Phone,Email,FullName,CreateDate")] AccountAdmin accountAdmin)
        {
            if (id != accountAdmin.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountAdmin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountAdminExists(accountAdmin.AccountId))
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
            return View(accountAdmin);
        }

        // GET: Admin/Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountAdmin = await _context.AccountAdmins
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (accountAdmin == null)
            {
                return NotFound();
            }

            return View(accountAdmin);
        }

        // POST: Admin/Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accountAdmin = await _context.AccountAdmins.FindAsync(id);
            _context.AccountAdmins.Remove(accountAdmin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountAdminExists(int id)
        {
            return _context.AccountAdmins.Any(e => e.AccountId == id);
        }
    }
}
