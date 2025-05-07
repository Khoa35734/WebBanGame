using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanGame.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebBanGame.Models.ViewModels;
namespace WebBanGame.Controllers
{

    public class AccountController : Controller
	{
        private readonly BanGameBanQuyenContext _context;
        public AccountController(BanGameBanQuyenContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            var account = _context.KhachHangs.ToList();
            return View(account);
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult DangKy(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra tài khoản đã tồn tại
            var existingUser = _context.KhachHangs
                .FirstOrDefault(kh => kh.TenTaiKhoan == model.TenTaiKhoan);

            if (existingUser != null)
            {
                ModelState.AddModelError("TenTaiKhoan", "Tài khoản đã tồn tại");
                return View(model);
            }

            // Tạo tài khoản mới
            var newAccount = new KhachHang
            {
                TenTaiKhoan = model.TenTaiKhoan,
                TenKh = model.TenBan,
                Diachi = model.DiaChi,
                Ngaysinh = DateOnly.FromDateTime(model.NgaySinh),
                Phone = int.Parse(model.SoDienThoai),
                Email = model.Email,
                MatKhau = model.MatKhau, // => nên mã hóa ở đây nếu có yêu cầu bảo mật
                SoDuTk = 0,
                CreateDate = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.KhachHangs.Add(newAccount);
            _context.SaveChanges();

            return RedirectToAction("DangNhap");
        }




        [HttpPost]
		public async Task<IActionResult> DangNhap(string username, string password)
		{
			// Tìm người dùng trong bảng KhachHangs
			var user = _context.KhachHangs
				.FirstOrDefault(u => u.TenTaiKhoan == username && u.MatKhau == password);

			if (user != null)
			{
				// Kiểm tra nếu là admin
				
				

				// Tạo danh sách claims cho người dùng
				var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.TenTaiKhoan),
			new Claim(ClaimTypes.NameIdentifier, user.MaKh.ToString()),
               new Claim("SoDuTk", user.SoDuTk?.ToString("F2") ?? "0.00") // Đảm bảo định dạng và tránh null

		};

				// Tạo ClaimsIdentity
				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				// Tạo ClaimsPrincipal
				var principal = new ClaimsPrincipal(identity);

				// Đăng nhập
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				// Điều hướng đến trang chủ
				return RedirectToAction("Index", "Home");
			}
			var adminUser = _context.AccountAdmins.FirstOrDefault(a => a.TaiKhoan == username && a.MatKhau == password);
			if (adminUser != null) // Nếu tìm thấy trong Tkadmin, nghĩa là admin
			{
				return RedirectToAction("Index", "Home", new { Area = "Admin" }); // Điều hướng đến Home/Index
			}
			// Nếu đăng nhập thất bại
			ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
			return View();
		}

		[Authorize]
        [HttpPost]

        public async Task<IActionResult> Logout()
        {
            // Xóa cookie của phiên đăng nhập
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Chuyển hướng người dùng về trang đăng nhập
            return RedirectToAction("Index", "Home");
        }
    }
}
