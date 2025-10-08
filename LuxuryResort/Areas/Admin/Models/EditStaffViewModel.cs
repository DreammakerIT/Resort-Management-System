using System.ComponentModel.DataAnnotations;

namespace LuxuryResort.Areas.Admin.Models
{
    public class EditStaffViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} ký tự.", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string? ConfirmPassword { get; set; }

        public List<string> AllRoles { get; set; } = new List<string>();
        public IList<string> UserRoles { get; set; } = new List<string>();
    }
}