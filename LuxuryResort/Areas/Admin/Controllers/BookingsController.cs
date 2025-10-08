using LuxuryResort.Data;
using LuxuryResort.Areas.Admin.Models; // Ensure this is the correct namespace for your ViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LuxuryResort.Areas.Admin.Controllers
{
    public class BookingsController : BaseController // Assuming BaseController is your admin base
    {
        private readonly LuxuryResortContext _context;

        public BookingsController(LuxuryResortContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                // To get the Room Type, you must go through RoomInstance first.
                // Use .ThenInclude() to load the nested property.
                .Include(b => b.RoomInstance)
                    .ThenInclude(ri => ri.Room) // <<< FIX #1: Correct include path
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new BookingAdminViewModel
                {
                    Id = b.Id,
                    GuestName = b.User.FullName,
                    RoomType = b.RoomInstance.Room.Type,
                    RoomNumber = b.RoomInstance.RoomNumber,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    PaymentMethod = b.PaymentMethod ?? "N/A",
                    TotalPrice = b.TotalAmount,
                    Status = b.Status,
                    BookingDate = b.BookingDate,
                    ConfirmationCode = b.ConfirmationCode,
                    PaymentCode = b.PaymentCode
                })
                .ToListAsync();

            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.RoomInstance)
                    .ThenInclude(ri => ri.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}