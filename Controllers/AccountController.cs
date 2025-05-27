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
using System.ComponentModel.DataAnnotations;
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
        [Authorize]
        [HttpGet]
        public IActionResult ThongTinTaiKhoan()
        {
            // Lấy username từ Claims
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("DangNhap");

            var khachHang = _context.KhachHangs
                .FirstOrDefault(kh => kh.TenTaiKhoan == username);

            if (khachHang == null)
                return NotFound();

            return View(khachHang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatTaiKhoan(KhachHang model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Dữ liệu không hợp lệ!";
                return View("ThongTinTaiKhoan", model);
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(k => k.MaKh == model.MaKh);
            if (khachHang == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy tài khoản để cập nhật!";
                return View("ThongTinTaiKhoan", model);
            }

            // Cập nhật các trường cho phép sửa
            khachHang.TenKh = model.TenKh;
            khachHang.Email = model.Email;
            khachHang.Phone = model.Phone;

            try
            {
                _context.SaveChanges();
                ViewBag.SuccessMessage = "Cập nhật thông tin thành công.";
            }
            catch
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi lưu dữ liệu.";
            }

            return View("ThongTinTaiKhoan", khachHang);
        }
        [HttpPost]
        public IActionResult DangKy(string username, string password, string fullname, string phone, string email, DateTime? dob1, string address)
        {
            // Xử lý logic đăng ký tài khoản
            if (ModelState.IsValid)
            {
                DateTime dob = dob1 ?? DateTime.Now; // Nếu dob1 null, sử dụng ngày hiện tại

                // Kiểm tra xem tên tài khoản đã tồn tại chưa
                var existingUser = _context.KhachHangs.FirstOrDefault(u => u.TenTaiKhoan == username);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage("TenTaiKhoan", "Tên tài khoản đã tồn tại.");
                    return View();
                }

                // Kiểm tra định dạng email
                if (!string.IsNullOrEmpty(email) && !new EmailAddressAttribute().IsValid(email))
                {
                    ViewBag.ErrorMessage("Email", "Địa chỉ email không hợp lệ.");
                    return View();
                }
                // Kiểm tra định dạng số điện thoại (giả sử là số nguyên dương)
                if (!string.IsNullOrEmpty(phone) && !int.TryParse(phone, out _))
                {
                    ViewBag.ErrorMessage("Phone", "Số điện thoại không hợp lệ.");
                    return View();
                }
                // Kiểm tra định dạng địa chỉ (giả sử không rỗng)
                if (string.IsNullOrEmpty(address))
                {
                    ViewBag.ErrorMessage("Diachi", "Địa chỉ không được để trống.");
                    return View();
                }
                // Kiểm tra định dạng họ tên (giả sử không rỗng)
                if (string.IsNullOrEmpty(fullname))
                {
                    ViewBag.ErrorMessage("TenKh", "Họ tên không được để trống.");
                    return View();
                }
                // Kiểm tra định dạng mật khẩu (giả sử không rỗng và có độ dài tối thiểu)
                if (string.IsNullOrEmpty(password) || password.Length < 6)
                {
                    ViewBag.ErrorMessage("MatKhau", "Mật khẩu phải có ít nhất 6 ký tự.");
                    return View();
                }
                // Kiểm tra định dạng ngày sinh (giả sử không rỗng và trong khoảng hợp lệ)
                if (dob > DateTime.Now || dob < DateTime.Now.AddYears(-100))
                {
                    ViewBag.ErrorMessage("Ngaysinh", "Ngày sinh không hợp lệ.");
                    return View();
                }
                // Kiểm tra định dạng tên tài khoản (giả sử không rỗng và có độ dài tối thiểu)
                if (string.IsNullOrEmpty(username) || username.Length < 3)
                {
                    ViewBag.ErrorMessage("TenTaiKhoan", "Tên tài khoản phải có ít nhất 3 ký tự.");
                    return View();
                }

                // Tạo tài khoản mới
                var newUser = new KhachHang
                {
                    TenTaiKhoan = username,
                    MatKhau = password, // Lưu ý: Mã hóa mật khẩu trước khi lưu
                    TenKh = fullname,
                    Phone = int.Parse(phone),
                    Email = email,
                    Ngaysinh = DateOnly.FromDateTime(dob), // Sử dụng FromDateTime để chuyển đổi DateTime thành DateOnly
                    SoDuTk = 0, // Hoặc giá trị mặc định khác
                    Diachi = address,
                };

                // Lưu vào database (giả sử bạn đã cấu hình DbContext)
                using (var db = new BanGameBanQuyenContext())
                {
                    db.KhachHangs.Add(newUser);
                    db.SaveChanges();
                }

                // Gửi thông báo đăng ký thành công
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";

                // Chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap");
            }

            // Nếu có lỗi, load lại form đăng ký
            return View();
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
		public IActionResult NapTien() => View();
        // AJAX - Nhận số tiền từ client, trả về URL QR code (JSON)
        [HttpPost]
        public IActionResult TaoLenhNapTien([FromBody] NapTienRequest req)
        {
            if (req == null || req.Amount <= 0)
            {
                return Json(new { success = false, message = "Số tiền không hợp lệ" });
            }

            var qrUrl = GenerateStaticVietQR(req.Amount);

            // Có thể sinh note chuyển khoản theo user
            var note = $"Nap tien {User.Identity?.Name ?? "user"} {req.Amount:N0}";

            return Json(new { success = true, qrUrl, amount = req.Amount, note });
        }

        // Trang hiển thị QR code (nếu dùng View thay vì modal)
        public IActionResult QRPayment(decimal amount)
        {
            var qrUrl = GenerateStaticVietQR(amount);

            var model = new QRViewModel
            {
                Amount = amount,
                QRUrl = qrUrl
            };

            return View(model);
        }

        // Người dùng xác nhận đã thanh toán (có thể gọi qua AJAX hoặc form)
        [HttpPost]
        public IActionResult XacNhanDaThanhToan(decimal amount)
        {
            // TODO: Có thể lưu lại giao dịch chờ xác nhận ở đây
            TempData["SuccessMessage"] = $"Đã ghi nhận yêu cầu thanh toán {amount:N0}đ. Vui lòng đợi xác nhận.";
            return RedirectToAction("Index");
        }

        // Sinh link QR code VietQR (static)
        private string GenerateStaticVietQR(decimal amount)
        {
            // Thông tin tài khoản nhận tiền
            string bankCode = "BIDV";
            string accountNumber = "5321335313";
            string accountName = "PHAM MINH KHOA";

            // Lưu ý: accountName cần không dấu, viết hoa
            string note = $"Nap tien {User.Identity?.Name ?? "user"} {amount:N0}";

            // Dùng dịch vụ VietQR để tạo link (hoặc API khác nếu muốn)
            // Tham khảo: https://img.vietqr.io/image/{bankCode}-{accountNumber}-qr_only.png?amount={amount}&addInfo={note}&accountName={accountName}
            // Lưu ý: accountName và note cần URL encode

            string qrUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-qr_only.png?amount={amount}&addInfo={Uri.EscapeDataString(note)}&accountName={Uri.EscapeDataString(accountName)}";

            return qrUrl;
        }
    }
}
