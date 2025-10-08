namespace LuxuryResort.Areas.Admin.Models
{
    public class StaffViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; } // Thêm Số điện thoại
        public bool EmailConfirmed { get; set; } // Thêm trạng thái xác thực email
        public DateTimeOffset? LockoutEnd { get; set; } // Thêm trạng thái khóa tài khoản
        public IEnumerable<string> Roles { get; set; }
    }
}