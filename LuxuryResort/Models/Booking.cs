// Models/Booking.cs
using LuxuryResort.Areas.Admin.Models;
using LuxuryResort.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxuryResort.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Khóa ngoại đến bảng User
        [ForeignKey("UserId")]
        public LuxuryResortUser User { get; set; }

        [Required]
        public int RoomInstanceId { get; set; }

        [ForeignKey("RoomInstanceId")]
        public virtual RoomInstance RoomInstance { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public int Adults { get; set; }

        public int Children { get; set; }

        [Required]
        public decimal TotalAmount { get; set; } // Tổng số tiền

        public string? SpecialRequest { get; set; } // Yêu cầu đặc biệt (có thể null)

        public string? PaymentMethod { get; set; } // "card", "hotel", "payment_code"

        public string Status { get; set; } // Ví dụ: "Confirmed", "Pending", "Cancelled", "Payment_Pending"

        [Required]
        public DateTime BookingDate { get; set; } // Ngày thực hiện đặt phòng

        public string? ConfirmationCode { get; set; } // Mã xác nhận

        public string? PaymentCode { get; set; } // Mã code thanh toán (6 chữ số)

        public DateTime? PaymentCodeExpiry { get; set; } // Thời hạn mã code thanh toán
    }
}