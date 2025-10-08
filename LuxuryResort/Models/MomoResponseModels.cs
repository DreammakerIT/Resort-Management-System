// Models/MomoResponseModels.cs
namespace LuxuryResort.Models
{
    public class MomoCreatePaymentResponseModel
    {
        public string payUrl { get; set; }
        public int resultCode { get; set; }
        // Thêm các thuộc tính khác nếu cần
    }

    public class MomoExecuteResponseModel
    {
        // Model này sẽ dùng cho việc xác nhận giao dịch sau này
        public int ResultCode { get; set; }
        public string Message { get; set; }
        // Thêm các thuộc tính khác nếu cần
    }
}