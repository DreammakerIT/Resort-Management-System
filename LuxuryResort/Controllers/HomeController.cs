using System.Diagnostics;
using LuxuryResort.Data;
using LuxuryResort.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuxuryResort.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        

        
        public HomeController(ILogger<HomeController> logger, LuxuryResortContext context)
        {
            _logger = logger;
            _context = context; // Gán giá trị cho _context
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách tất cả các phòng từ database
            var allRooms = await _context.Rooms.OrderBy(r => r.PricePerNight).ToListAsync();

            // Gửi danh sách phòng đến View
            return View(allRooms);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TimHieuThem()
        {
            return View();
        }

        public IActionResult Dining()
        {
            return View();
        }

        public IActionResult Facilities()
        {
            return View();
        }
        public IActionResult Pool()
        {
            return View();
        }

        public IActionResult Offers()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> RoomDetails()
        {
            var roomModel = await _context.Rooms.FirstOrDefaultAsync(r => r.Type == "Deluxe Ocean View");
            if (roomModel == null) return NotFound();
            return View(roomModel);
        }



        public async Task<IActionResult> PresidentialSuite()
        {
            var roomModel = await _context.Rooms.FirstOrDefaultAsync(r => r.Type == "Presidential Suite");
            if (roomModel == null) return NotFound();
            return View(roomModel);
        }

        public async Task<IActionResult> BungalowGarden()
        {
            var roomModel = await _context.Rooms
                                          .FirstOrDefaultAsync(r => r.Type == "Garden View Bungalow");
            if (roomModel == null) return NotFound();
            return View(roomModel);
        }

        public async Task<IActionResult> SuperiorGardenView()
        {
            var roomModel = await _context.Rooms
                                          .FirstOrDefaultAsync(r => r.Type == "Superior Garden View");
            if (roomModel == null) return NotFound();

            // Tính toán availability cho phòng này
            var roomInstances = await _context.RoomInstances
                .Where(ri => ri.RoomId == roomModel.Id)
                .ToListAsync();

            var today = DateTime.Today;
            var bookedRooms = await _context.Bookings
                .Where(b => roomInstances.Select(ri => ri.Id).Contains(b.RoomInstanceId) && 
                           b.Status != "Cancelled" &&
                           b.CheckInDate <= today && 
                           b.CheckOutDate > today)
                .CountAsync();

            var availableRooms = Math.Max(0, roomInstances.Count - bookedRooms);

            ViewBag.AvailableRooms = availableRooms;
            ViewBag.TotalRooms = roomInstances.Count;
            ViewBag.BookedRooms = bookedRooms;

            return View(roomModel);
        }

        public async Task<IActionResult> DeluxeCityView()
        {
            var roomModel = await _context.Rooms
                                          .FirstOrDefaultAsync(r => r.Type == "Deluxe City View");
            if (roomModel == null) return NotFound();

            // Tính toán availability cho phòng này
            var roomInstances = await _context.RoomInstances
                .Where(ri => ri.RoomId == roomModel.Id)
                .ToListAsync();

            var today = DateTime.Today;
            var bookedRooms = await _context.Bookings
                .Where(b => roomInstances.Select(ri => ri.Id).Contains(b.RoomInstanceId) && 
                           b.Status != "Cancelled" &&
                           b.CheckInDate <= today && 
                           b.CheckOutDate > today)
                .CountAsync();

            var availableRooms = Math.Max(0, roomInstances.Count - bookedRooms);

            ViewBag.AvailableRooms = availableRooms;
            ViewBag.TotalRooms = roomInstances.Count;
            ViewBag.BookedRooms = bookedRooms;

            return View(roomModel);
        }

        private readonly LuxuryResortContext _context;

        public async Task<IActionResult> PrivatePoolVilla()
        {
            // Tìm phòng có tên là "Private Pool Villa" trong database
            var roomModel = await _context.Rooms
                                          .FirstOrDefaultAsync(r => r.Type == "Private Pool Villa");
            
            if (roomModel == null)
            {
                return NotFound(); // Không tìm thấy phòng trong DB
            }
            
            // Gửi đối tượng roomModel (chứa đầy đủ thông tin) đến View
            return View(roomModel);
            
            }

        public async Task<IActionResult> OceanfrontVilla()
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Type == "Oceanfront Residence Villa");
            if (room == null) return NotFound();
            return View(room);
        }

        public async Task<IActionResult> Rooms()
        {
            // Lấy TẤT CẢ các phòng từ database và sắp xếp theo giá
            var allRooms = await _context.Rooms.OrderBy(r => r.PricePerNight).ToListAsync();

            // Tính toán số lượng phòng còn lại cho mỗi hạng phòng
            var roomsWithAvailability = new List<RoomWithAvailabilityViewModel>();

            foreach (var room in allRooms)
            {
                // Lấy tất cả room instances của hạng phòng này
                var roomInstances = await _context.RoomInstances
                    .Where(ri => ri.RoomId == room.Id)
                    .ToListAsync();

                // Đếm số phòng đã được đặt cho ngày hiện tại (không bao gồm cancelled)
                var today = DateTime.Today;
                var bookedRooms = await _context.Bookings
                    .Where(b => roomInstances.Select(ri => ri.Id).Contains(b.RoomInstanceId) && 
                               b.Status != "Cancelled" &&
                               b.CheckInDate <= today && 
                               b.CheckOutDate > today)
                    .CountAsync();

                var availableRooms = roomInstances.Count - bookedRooms;

                roomsWithAvailability.Add(new RoomWithAvailabilityViewModel
                {
                    Id = room.Id,
                    Type = room.Type,
                    Description = room.Description,
                    ImageUrl = room.ImageUrl,
                    PricePerNight = room.PricePerNight,
                    MaxOccupancy = room.MaxOccupancy,
                    MaxChildren = room.MaxChildren,
                    Area = room.Area,
                    TotalRooms = roomInstances.Count,
                    AvailableRooms = Math.Max(0, availableRooms), // Đảm bảo không âm
                    BookedRooms = bookedRooms
                });
            }

            // Gửi danh sách phòng với thông tin availability đến View
            return View(roomsWithAvailability);
        }


    }
}
