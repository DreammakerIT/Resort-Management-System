using LuxuryResort.Data;
using LuxuryResort.Models; // Ensure your ViewModels are in this namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System; // Required for DateTime
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Required for List

namespace LuxuryResort.Controllers
{
    public class AvailabilityController : Controller
    {
        private readonly LuxuryResortContext _context;

        public AvailabilityController(LuxuryResortContext context)
        {
            _context = context;
        }

        // GET: /Availability/Results
        public async Task<IActionResult> Results(DateTime checkin, DateTime checkout, int adults, int children)
        {
            // Basic validation
            if (checkin >= checkout)
            {
                // Handle invalid date range, maybe redirect to home with an error message
                return RedirectToAction("Index", "Home");
            }

            int totalGuests = adults + children;
            int totalNights = (checkout - checkin).Days;
            if (totalNights <= 0) totalNights = 1;

            // 1. Find the IDs of all specific room instances that are already booked during the selected timeframe.
            var bookedRoomInstanceIds = await _context.Bookings
                .Where(b => b.Status != "Cancelled" && checkin < b.CheckOutDate && checkout > b.CheckInDate)
                .Select(b => b.RoomInstanceId)
                .Distinct()
                .ToListAsync();

            // 2. Find all room types that meet the guest count requirement.
            // For each of these room types, check if there is at least one specific room instance that is NOT in the booked list.
            var availableRoomTypes = new List<AvailableRoomViewModel>();
            
            var roomTypes = await _context.Rooms
                .Where(r => (r.MaxOccupancy + r.MaxChildren) >= totalGuests)
                .ToListAsync();

            foreach (var room in roomTypes)
            {
                // Lấy tất cả room instances của hạng phòng này
                var roomInstances = await _context.RoomInstances
                    .Where(ri => ri.RoomId == room.Id)
                    .ToListAsync();

                // Đếm số phòng đã được đặt trong khoảng thời gian được chọn
                var bookedRoomsInPeriod = await _context.Bookings
                    .Where(b => roomInstances.Select(ri => ri.Id).Contains(b.RoomInstanceId) && 
                               b.Status != "Cancelled" &&
                               checkin < b.CheckOutDate && 
                               checkout > b.CheckInDate)
                    .CountAsync();

                var availableRooms = roomInstances.Count - bookedRoomsInPeriod;

                // Chỉ thêm vào danh sách nếu còn ít nhất 1 phòng trống
                if (availableRooms > 0)
                {
                    availableRoomTypes.Add(new AvailableRoomViewModel
                    {
                        RoomId = room.Id,
                        RoomType = room.Type,
                        Description = room.Description,
                        ImageUrl = room.ImageUrl,
                        MaxOccupancy = room.MaxOccupancy,
                        TotalPrice = room.PricePerNight * totalNights,
                        AvailableRooms = availableRooms,
                        TotalRooms = roomInstances.Count,
                        BookedRooms = bookedRoomsInPeriod
                    });
                }
            }

            var viewModel = new AvailabilityViewModel
            {
                CheckInDate = checkin,
                CheckOutDate = checkout,
                Adults = adults,
                Children = children,
                TotalNights = totalNights,
                AvailableRooms = availableRoomTypes // Now this list is accurate
            };

            return View(viewModel);
        }
    }
}