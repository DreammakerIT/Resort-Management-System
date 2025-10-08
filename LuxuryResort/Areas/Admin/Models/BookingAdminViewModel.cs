namespace LuxuryResort.Areas.Admin.Models
{
    public class BookingAdminViewModel
    {
        public int Id { get; set; }
        public string GuestName { get; set; }
        public string RoomType { get; set; } // e.g., "Deluxe Ocean View"
        public string RoomNumber { get; set; } // e.g., "101"
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string PaymentMethod { get; set; } // "payment_code", "hotel"
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string? ConfirmationCode { get; set; }
        public string? PaymentCode { get; set; }
    }
}