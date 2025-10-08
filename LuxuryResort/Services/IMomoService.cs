// Services/IMomoService.cs
using LuxuryResort.Models;
using Microsoft.AspNetCore.Http; // Cần cho IQueryCollection
using System.Threading.Tasks;

public interface IMomoService // Đảm bảo interface là public
{
    Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(Booking booking);
    bool ValidateSignature(string amount, string message, string orderId, string orderInfo, string orderType, string payType, long requestId, int resultCode, long transId, string responseTime);

    // Tạm thời để trống phương thức này, sẽ hoàn thiện sau
    // MomoExecuteResponseModel PaymentExecute(IQueryCollection collection); 
}