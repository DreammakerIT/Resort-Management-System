// Controllers/FakeGatewayController.cs
using Microsoft.AspNetCore.Mvc;
using System.Web; // Thêm using này để dùng HttpUtility

namespace LuxuryResort.Controllers
{
    // Controller này không cần Authorize
    public class FakeGatewayController : Controller
    {
        [HttpGet]
        public IActionResult Index(string partnerCode, string orderId, string amount, string orderInfo, string redirectUrl, string ipnUrl, string signature)
        {
            // Chuyển toàn bộ dữ liệu nhận được sang cho View
            ViewBag.PartnerCode = partnerCode;
            ViewBag.OrderId = orderId;
            ViewBag.Amount = amount;
            ViewBag.OrderInfo = orderInfo;

            // URL để quay về sau khi mô phỏng thanh toán
            ViewBag.RedirectUrl = redirectUrl;
            ViewBag.IpnUrl = ipnUrl;
            ViewBag.Signature = signature; // Chữ ký giả lập

            return View();
        }
    }
}