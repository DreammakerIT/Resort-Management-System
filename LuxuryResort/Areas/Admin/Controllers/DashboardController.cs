using LuxuryResort.Areas.Admin.Models; // Đảm bảo đúng namespace của ViewModel
using LuxuryResort.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System; // Cần cho DateTime
using System.Collections.Generic; // Cần cho Dictionary
using System.Linq;
using System.Threading.Tasks;

namespace LuxuryResort.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : BaseController // Giả sử BaseController là controller cơ sở của bạn
    {
        private readonly LuxuryResortContext _context;

        public DashboardController(LuxuryResortContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // --- Các truy vấn thống kê (giữ nguyên, đã đúng) ---
            var todaysRevenue = await _context.Bookings
                .Where(b => b.BookingDate >= today && b.BookingDate < tomorrow && 
                           (b.Status == "Confirmed" || b.Status == "Payment_Pending" || b.Status == "Completed"))
                .SumAsync(b => b.TotalAmount);

            var newBookingsCount = await _context.Bookings
                .CountAsync(b => b.BookingDate >= today && b.BookingDate < tomorrow);

            var checkInsCount = await _context.Bookings
                .CountAsync(b => b.CheckInDate.Date == today && 
                               (b.Status == "Confirmed" || b.Status == "Payment_Pending" || b.Status == "Completed"));

            var checkOutsCount = await _context.Bookings
                .CountAsync(b => b.CheckOutDate.Date == today && 
                               (b.Status == "Confirmed" || b.Status == "Payment_Pending" || b.Status == "Completed"));

            // --- SỬA LỖI TRUY VẤN CÁC ĐẶT PHÒNG GẦN ĐÂY ---
            var recentBookings = await _context.Bookings
                .Include(b => b.User)
                // Sửa lại đường dẫn Include cho đúng
                .Include(b => b.RoomInstance)         // <<<< SỬA LỖI #1
                    .ThenInclude(ri => ri.Room)   // <<<< SỬA LỖI #1
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .Select(b => new BookingInfoViewModel // ViewModel này bạn cần tạo nếu chưa có
                {
                    GuestName = b.User.FullName,
                    // Sửa lại cách truy cập thuộc tính Type
                    RoomType = b.RoomInstance.Room.Type, // <<<< SỬA LỖI #2
                    CheckInDate = b.CheckInDate,
                    Status = b.Status
                })
                .ToListAsync();

            // --- Phần tính toán tình trạng phòng (ví dụ) ---
            var totalRoomInstances = await _context.RoomInstances.CountAsync();
            var occupiedRoomInstances = await _context.Bookings
                .Where(b => b.CheckInDate.Date <= today && b.CheckOutDate.Date > today && 
                           (b.Status == "Confirmed" || b.Status == "Payment_Pending" || b.Status == "Completed"))
                .CountAsync();

            var roomStatusCounts = new Dictionary<string, int>
            {
                { "Đang có khách", occupiedRoomInstances },
                { "Trống", totalRoomInstances - occupiedRoomInstances }
                // Bạn có thể thêm các trạng thái khác như "Đang dọn dẹp", "Bảo trì" nếu có trong CSDL
            };

            // --- Dữ liệu biểu đồ: phân bổ theo trạng thái ---
            var bookingsByStatus = await _context.Bookings
                .GroupBy(b => b.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            // --- Dữ liệu biểu đồ: doanh thu 7 ngày gần nhất ---
            var last7DaysLabels = new List<string>();
            var last7DaysRevenue = new List<decimal>();
            for (int i = 6; i >= 0; i--)
            {
                var day = today.AddDays(-i);
                var next = day.AddDays(1);
                var revenue = await _context.Bookings
                    .Where(b => b.BookingDate >= day && b.BookingDate < next &&
                                (b.Status == "Confirmed" || b.Status == "Payment_Pending" || b.Status == "Completed"))
                    .SumAsync(b => b.TotalAmount);
                last7DaysLabels.Add(day.ToString("dd/MM"));
                last7DaysRevenue.Add(revenue);
            }

            // Tạo ViewModel và gán dữ liệu
            var viewModel = new DashboardViewModel // ViewModel này bạn cần tạo nếu chưa có
            {
                TodaysRevenue = todaysRevenue,
                NewBookingsCount = newBookingsCount,
                CheckInsCount = checkInsCount,
                CheckOutsCount = checkOutsCount,
                RecentBookings = recentBookings,
                RoomStatusCounts = roomStatusCounts,
                BookingsByStatus = bookingsByStatus,
                Last7DaysLabels = last7DaysLabels,
                Last7DaysRevenue = last7DaysRevenue
            };

            return View(viewModel);
        }
    }
}