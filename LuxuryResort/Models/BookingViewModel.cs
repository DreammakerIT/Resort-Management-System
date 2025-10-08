// Models/BookingViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuxuryResort.Models
{
    public class BookingViewModel
    {
        // --- Dữ liệu để hiển thị trên form ---
        public Room SelectedRoom { get; set; }
        public IEnumerable<Room> AllRooms { get; set; }

        // --- Dữ liệu người dùng sẽ gửi lên ---
        [Required]
        public int SelectedRoomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Số người lớn phải từ 1 đến 10")]
        public int Adults { get; set; }

        [Range(0, 10, ErrorMessage = "Số trẻ em phải từ 0 đến 10")]
        public int Children { get; set; }

        // Thông tin người đặt
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        public string? SpecialRequest { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "card"; // "card", "hotel", "payment_code"
        
        // Thông tin cho mã code thanh toán
        public string? PaymentCode { get; set; }
        public DateTime? PaymentCodeExpiry { get; set; }
    }
}