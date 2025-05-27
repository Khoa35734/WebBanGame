using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebBanGame.Models;

namespace WebBanGame.Controllers
{
    public class NapTienController : Controller
        {
        private readonly BanGameBanQuyenContext _context;
        public NapTienController(BanGameBanQuyenContext context)
        {
            _context = context;
        }
        // GET: NapTien
        [HttpGet]
    
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult NapTienQR(decimal amount)
        {
            var qrUrl = GenerateStaticVietQR(amount);

            var model = new QRViewModel
            {
                Amount = amount,
                QRUrl = qrUrl
            };

            return View("QRPayment", model);
        }
        public async Task<IActionResult> LichSuNapTien()
        {
            int maKh = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value); ; // Lấy mã khách hàng đang đăng nhập
            var lichSuNap = await _context.NapTiens
                .Where(n => n.IdUser == maKh)
                .OrderByDescending(n => n.NgayNap)
                .Select(n => new LichSuNapTienViewModel
                {
                    IdNapTien = n.IdNapTien,
                    SoTienNap = n.SoTienNap,
                    TrangThai = n.TrangThai,
                    NgayNap = n.NgayNap,
                    BankTransactionId = n.BankTransactionId
                })
                .ToListAsync();

            return View(lichSuNap);
        }
        [HttpPost]
        public IActionResult XacNhanDaThanhToan(decimal amount)
        {
            TempData["SuccessMessage"] = $"Đã ghi nhận yêu cầu thanh toán {amount:N0}đ. Vui lòng đợi xác nhận.";
            return RedirectToAction("Index");
        }

        private string GenerateStaticVietQR(decimal amount)
        {
            string bankCode = "VCB";
            string accountNumber = "0123456789";
            string accountName = "CONG TY ABC";
            string description = $"Nap tien {amount:N0}";

            return $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact2.png?amount={amount}&addInfo={description}&accountName={Uri.EscapeDataString(accountName)}";
        }
    }
}
