namespace LuxuryResort.Models
{
    public class AvailabilityViewModel
    {
        // Thông tin tìm kiếm để hiển thị lại cho người dùng
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int TotalNights { get; set; }

        // Danh sách các phòng phù hợp
        public List<AvailableRoomViewModel> AvailableRooms { get; set; }
    }

    public class AvailableRoomViewModel
    {
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public string Description { get; set; } // Thêm mô tả ngắn
        public string ImageUrl { get; set; } // Thêm ảnh
        public int MaxOccupancy { get; set; }
        public decimal TotalPrice { get; set; } // Tổng giá cho cả kỳ nghỉ
        public int AvailableRooms { get; set; } // Số phòng còn trống
        public int TotalRooms { get; set; } // Tổng số phòng
        public int BookedRooms { get; set; } // Số phòng đã đặt
    }
}