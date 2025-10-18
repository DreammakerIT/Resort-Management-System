using LuxuryResort.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace LuxuryResort.Models
{
    public class ProfileViewModel
    {
        public LuxuryResortUser User { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public int TotalBookings { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class ProfileEditViewModel
    {
        public string Id { get; set; }
        
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Quốc tịch")]
        public string? Nationality { get; set; }

        [Display(Name = "Số hộ chiếu")]
        public string? PassportNumber { get; set; }
    }

    public class PreferencesViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Loại phòng ưa thích")]
        public string? PreferredRoomType { get; set; }

        [Display(Name = "Yêu cầu đặc biệt")]
        [DataType(DataType.MultilineText)]
        public string? SpecialRequests { get; set; }

        [Display(Name = "Đồng ý nhận thông tin marketing")]
        public bool MarketingConsent { get; set; }

        [Display(Name = "Đăng ký nhận bản tin")]
        public bool NewsletterSubscription { get; set; }
    }
}
