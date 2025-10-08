namespace LuxuryResort.Areas.Admin.Models
{
    public class CustomerViewModel
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsLocked { get; set; } // Trạng thái có bị khóa hay không
        public DateTime? RegistrationDate { get; set; }
    }
}
