// Models/RoomInstance.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LuxuryResort.Models;
using LuxuryResort.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LuxuryResort.Areas.Admin.Models
{
    public class RoomInstance
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số phòng")]
        [Display(Name = "Số phòng")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hạng phòng")]
        [Display(Name = "Hạng phòng")]
        public int RoomId { get; set; }

        // --- CÁC THUỘC TÍNH KHÔNG CẦN [Required] TRÊN FORM ---

        [Display(Name = "Tình trạng")]
        public string? Status { get; set; } // Controller sẽ tự gán

        [ForeignKey("RoomId")]
        [ValidateNever]
        public virtual Room? Room { get; set; } // Navigation property

        [ValidateNever]
        public virtual ICollection<Booking>? Bookings { get; set; } // Navigation collection
    }
}