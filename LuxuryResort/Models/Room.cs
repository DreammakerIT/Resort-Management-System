using System.ComponentModel.DataAnnotations;
using LuxuryResort.Areas.Admin.Models;

namespace LuxuryResort.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên hạng phòng")]
        public string Type { get; set; } // Ví dụ: "Deluxe Ocean View"

        [Required]
        [Display(Name = "Tổng số phòng")]
        public int TotalRooms { get; set; }

        [Display(Name = "Mô tả ngắn")]
        public string? Description { get; set; }

        [Display(Name = "URL Hình ảnh chính")]
        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Giá mỗi đêm")]
        [DataType(DataType.Currency)]
        public decimal PricePerNight { get; set; }

        [Required]
        [Display(Name = "Sức chứa tối đa (Người Lớn)")]
        public int MaxOccupancy { get; set; }


        [Required]
        [Display(Name = "Số trẻ em tối đa (dưới 15 tuổi)")]


        public int MaxChildren { get; set; }
        private int area;



        public int GetArea()
        {
            return area;
        }

        public void SetArea(int value)
        {
            area = value;
        }

        [Required]
        [Display(Name = "Diện tích (m²)")]
        public int Area { get; set; }

        // Các thuộc tính này không cần hiển thị trên form
        public string? BedType { get; set; } // Ví dụ: "1 Giường King"
        public string? ViewType { get; set; } // Ví dụ: "Hướng biển"
        public virtual ICollection<RoomInstance> RoomInstances { get; set; }
    }
}