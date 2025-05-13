using Microsoft.AspNetCore.Mvc;
using WebBanGame.Models;
using System;

namespace WebBanGame.Controllers
{
    public class NapTienController : Controller
    {
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
