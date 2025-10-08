namespace LuxuryResort.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public decimal TodaysRevenue { get; set; }
        public int NewBookingsCount { get; set; }
        public int CheckInsCount { get; set; }
        public int CheckOutsCount { get; set; }
        public List<BookingInfoViewModel> RecentBookings { get; set; }
        public Dictionary<string, int> RoomStatusCounts { get; set; }
    }

    public class BookingInfoViewModel
    {
        public string GuestName { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public string Status { get; set; }
    }
}