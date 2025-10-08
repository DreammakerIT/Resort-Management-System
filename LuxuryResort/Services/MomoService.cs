using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LuxuryResort.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp; // Đảm bảo đã using RestSharp

namespace LuxuryResort.Services // Đảm bảo đúng namespace
{
    public class MomoService : IMomoService
    {
        private readonly MomoSettings _momoSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MomoService(IOptions<MomoSettings> momoSettings, IHttpContextAccessor httpContextAccessor)
        {
            _momoSettings = momoSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(Booking booking)
        {
            if (!_momoSettings.UseFakeGateway)
            {
                var orderId = Guid.NewGuid().ToString();
                var amount = ((long)booking.TotalAmount).ToString();
                var orderInfo = $"Thanh toán đặt phòng #{booking.Id}";

                var rawHash = $"accessKey={_momoSettings.AccessKey}&amount={amount}&extraData=&ipnUrl={_momoSettings.IpnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={_momoSettings.PartnerCode}&redirectUrl={_momoSettings.RedirectUrl}&requestId={orderId}&requestType=captureWallet";
                var signature = SignHmacSHA256(rawHash, _momoSettings.SecretKey);

                var client = new RestClient(_momoSettings.Endpoint);
                var request1 = new RestRequest() { Method = RestSharp.Method.Post };
                request1.AddHeader("Content-Type", "application/json; charset=UTF-8");

                var requestBody = new
                {
                    partnerCode = _momoSettings.PartnerCode,
                    accessKey = _momoSettings.AccessKey,
                    requestId = orderId,
                    amount = amount,
                    orderId = orderId,
                    orderInfo = orderInfo,
                    redirectUrl = _momoSettings.RedirectUrl,
                    ipnUrl = _momoSettings.IpnUrl,
                    extraData = "",
                    requestType = "captureWallet",
                    signature = signature,
                    lang = "vi"
                };

                request1.AddParameter("application/json", JsonConvert.SerializeObject(requestBody), ParameterType.RequestBody);
                var response = await client.ExecuteAsync(request1);
                var parsed = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
                return parsed;
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : string.Empty;

            var fakePaymentUrl = $"{baseUrl}/FakeGateway?partnerCode=DUMMY_PARTNER" +
                        $"&orderId={Guid.NewGuid().ToString()}" +
                        $"&amount={booking.TotalAmount}" +
                        $"&orderInfo={HttpUtility.UrlEncode($"Thanh toan cho don hang {booking.Id}")}" +
                        $"&redirectUrl={HttpUtility.UrlEncode(_momoSettings.RedirectUrl)}" +
                        $"&ipnUrl={HttpUtility.UrlEncode(_momoSettings.IpnUrl)}" +
                        $"&signature=FAKE_SIGNATURE";

            var fakeResponse = new MomoCreatePaymentResponseModel
            {
                payUrl = fakePaymentUrl,
                resultCode = 0 // Giả lập việc tạo yêu cầu thành công
            };

            // Dùng Task.FromResult để trả về một Task hoàn thành ngay lập tức
            return await Task.FromResult(fakeResponse);
        }

        // SỬA LỖI CS0161: Đảm bảo mọi đường đi đều trả về giá trị
        private string SignHmacSHA256(string message, string key)
        {
            string signature = string.Empty; // Khai báo biến ở ngoài
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage);
                hex = hex.Replace("-", "").ToLower();
                signature = hex; // Gán giá trị
            }
            return signature; // Trả về giá trị ở cuối hàm
        }

        // Services/MomoService.cs

        public bool ValidateSignature(string amount, string message, string orderId, string orderInfo, string orderType, string payType, long requestId, int resultCode, long transId, string responseTime)
        {
            // Chuỗi dữ liệu gốc MoMo gửi về để tạo chữ ký
            var rawHash = $"accessKey={_momoSettings.AccessKey}&amount={amount}&extraData=&message={message}&orderId={orderId}&orderInfo={orderInfo}&orderType={orderType}&partnerCode={_momoSettings.PartnerCode}&payType={payType}&requestId={requestId}&responseTime={responseTime}&resultCode={resultCode}&transId={transId}";

            var momoSignature = SignHmacSHA256(rawHash, _momoSettings.SecretKey);

            // Lấy chữ ký từ query string do MoMo gửi về
            var requestSignature = _httpContextAccessor.HttpContext.Request.Query["signature"];

            // So sánh chữ ký bạn tạo ra với chữ ký MoMo gửi về
            return momoSignature.Equals(requestSignature);
        }
    }
}