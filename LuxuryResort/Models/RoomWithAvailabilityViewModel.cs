namespace LuxuryResort.Models
{
    public class RoomWithAvailabilityViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
        public int MaxChildren { get; set; }
        public int Area { get; set; }
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; } // Số phòng còn trống
        public int BookedRooms { get; set; } // Số phòng đã đặt
    }
}



