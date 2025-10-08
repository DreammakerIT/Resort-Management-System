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
                .Where(b => b.BookingDate >= today && b.BookingDate < tomorrow && b.Status == "Confirmed")
                .SumAsync(b => b.TotalAmount);

            var newBookingsCount = await _context.Bookings
                .CountAsync(b => b.BookingDate >= today && b.BookingDate < tomorrow);

            var checkInsCount = await _context.Bookings
                .CountAsync(b => b.CheckInDate.Date == today && b.Status == "Confirmed");

            var checkOutsCount = await _context.Bookings
                .CountAsync(b => b.CheckOutDate.Date == today && b.Status == "Confirmed");

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
                .Where(b => b.CheckInDate.Date <= today && b.CheckOutDate.Date > today && b.Status == "Confirmed")
                .CountAsync();

            var roomStatusCounts = new Dictionary<string, int>
            {
                { "Đang có khách", occupiedRoomInstances },
                { "Trống", totalRoomInstances - occupiedRoomInstances }
                // Bạn có thể thêm các trạng thái khác như "Đang dọn dẹp", "Bảo trì" nếu có trong CSDL
            };

            // Tạo ViewModel và gán dữ liệu
            var viewModel = new DashboardViewModel // ViewModel này bạn cần tạo nếu chưa có
            {
                TodaysRevenue = todaysRevenue,
                NewBookingsCount = newBookingsCount,
                CheckInsCount = checkInsCount,
                CheckOutsCount = checkOutsCount,
                RecentBookings = recentBookings,
                RoomStatusCounts = roomStatusCounts
            };

            return View(viewModel);
        }
    }
}