using System.ComponentModel.DataAnnotations;

namespace LuxuryResort.Areas.Admin.Models
{
    public class CreateStaffViewModel
    {
        [Required]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10, ErrorMessage = "Số Điện Thoại Chưa Đủ Số")]
        [Display(Name = "Số Điện Thoại")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 6)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Vai trò")]
        public string Role { get; set; }
    }
}