using System;
using System.Linq;
using System.Threading.Tasks;
using LuxuryResort.Areas.Identity.Data;
using LuxuryResort.Data;
using LuxuryResort.Models;
using LuxuryResort.Services.Vnpay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuxuryResort.Controllers
{
    [Authorize]
        public class BookingController : Controller
    {
        private readonly LuxuryResortContext _context;
        private readonly UserManager<LuxuryResortUser> _userManager;
        private readonly IVnPayService _vnPayService;
        private readonly Random _random = new Random();
            private readonly IConfiguration _configuration;

        public BookingController(LuxuryResortContext context, UserManager<LuxuryResortUser> userManager, IConfiguration configuration, IVnPayService vnPayService)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _vnPayService = vnPayService;
        }

        // [HttpGet] để hiển thị trang đặt phòng
        [HttpGet]
        public async Task<IActionResult> Create(int roomId, DateTime? checkin, DateTime? checkout, int adults = 1, int children = 0)
        {
            var selectedRoom = await _context.Rooms.FindAsync(roomId);
            if (selectedRoom == null)
            {
                return NotFound();
            }

            var allRooms = await _context.Rooms.OrderBy(r => r.PricePerNight).ToListAsync();

            DateTime validCheckin = checkin.HasValue && checkin.Value.Date >= DateTime.Today ? checkin.Value : DateTime.Today;
            DateTime validCheckout = checkout.HasValue && checkout.Value.Date > validCheckin.Date ? checkout.Value : validCheckin.AddDays(1);

            var user = await _userManager.GetUserAsync(User);

            var viewModel = new BookingViewModel
            {
                SelectedRoomId = roomId,
                SelectedRoom = selectedRoom,
                AllRooms = allRooms,
                CheckInDate = validCheckin,
                CheckOutDate = validCheckout,
                Adults = adults > 0 ? adults : 1,
                Children = children,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            model.SelectedRoom = await _context.Rooms.FindAsync(model.SelectedRoomId);

            // Fill missing customer info from current user to avoid validation blocks
            var currentUser = await _userManager.GetUserAsync(User);
            if (string.IsNullOrWhiteSpace(model.FullName)) model.FullName = currentUser?.FullName ?? string.Empty;
            if (string.IsNullOrWhiteSpace(model.Email)) model.Email = currentUser?.Email ?? string.Empty;
            if (string.IsNullOrWhiteSpace(model.Phone)) model.Phone = currentUser?.PhoneNumber ?? string.Empty;

            // Normalize inputs
            if (model.Adults <= 0) model.Adults = 1;
            if (model.CheckInDate == default) model.CheckInDate = DateTime.Today;
            if (model.CheckOutDate <= model.CheckInDate) model.CheckOutDate = model.CheckInDate.AddDays(1);

            // Do not block on ModelState; we guard essential fields ourselves

            try
            {
               var existingInstances = await _context.RoomInstances
                    .Where(ri => ri.RoomId == model.SelectedRoomId)
                    .ToListAsync();

                // If no room instances exist for this room type, auto-create one to unblock testing
                if (!existingInstances.Any())
                {
                    var newInstance = new Areas.Admin.Models.RoomInstance
                    {
                        RoomId = model.SelectedRoomId,
                        RoomNumber = $"AUTO-{model.SelectedRoomId}-1",
                        Status = "Available"
                    };
                    _context.RoomInstances.Add(newInstance);
                    await _context.SaveChangesAsync();
                    existingInstances = new List<Areas.Admin.Models.RoomInstance> { newInstance };
                }

                var availableRoomInstanceIds = existingInstances.Select(ri => ri.Id).ToList();

                var bookedRoomInstanceIds = await _context.Bookings
                    .Where(b => availableRoomInstanceIds.Contains(b.RoomInstanceId) &&
                                b.Status != "Cancelled" &&
                                model.CheckInDate < b.CheckOutDate &&
                                model.CheckOutDate > b.CheckInDate)
                    .Select(b => b.RoomInstanceId)
                    .ToListAsync();
               
                var firstAvailableRoomInstanceId = availableRoomInstanceIds.Except(bookedRoomInstanceIds).FirstOrDefault();
                                
                if (firstAvailableRoomInstanceId == 0)
                {
                    ModelState.AddModelError("", "We're sorry, this room type is fully booked for the selected dates.");
                    TempData["PaymentError"] = "Hạng phòng đã kín trong khoảng ngày bạn chọn. Vui lòng đổi ngày hoặc phòng.";
                    return View(model);
                }


                var user = await _userManager.GetUserAsync(User);
                var roomType = model.SelectedRoom; // Already fetched
                var numberOfNights = (int)(model.CheckOutDate - model.CheckInDate).TotalDays;

                var booking = new Booking
                {
                    UserId = user.Id,
                    RoomInstanceId = firstAvailableRoomInstanceId, // <<< Assign the available specific room
                    CheckInDate = model.CheckInDate,
                    CheckOutDate = model.CheckOutDate,
                    Adults = model.Adults,
                    Children = model.Children,
                    TotalAmount = roomType.PricePerNight * numberOfNights,
                    SpecialRequest = model.SpecialRequest,
                    PaymentMethod = model.PaymentMethod,
                    Status = "Pending",
                    BookingDate = DateTime.Now
                };

                if (model.PaymentMethod == "vnpay")
                {
                    booking.Status = "Payment_Pending";
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    var payModel = new Models.Vnpay.PaymentInformationModel
                    {
                        BookingId = booking.Id,
                        Amount = (double)booking.TotalAmount,
                        OrderDescription = $"Booking {booking.Id}",
                        OrderType = "room",
                        Name = user.FullName
                    };

                    var url = _vnPayService.CreatePaymentUrl(payModel, HttpContext);
                    return Redirect(url);
                }
                else // Pay at hotel
                {
                    booking.Status = "Completed";
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();
                    // Tạo mã xác nhận và chuyển luôn sang trang thành công
                    booking.ConfirmationCode = "MLR-" + booking.Id.ToString("D6");
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Success", new { id = booking.Id });
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.RoomInstance)
                    .ThenInclude(ri => ri.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null || (user != null && booking.UserId != user.Id && !User.IsInRole("Admin")))
            {
                return NotFound();
            }

            // Generate VietQR URL (scanable by MoMo and most VN banks)
            var bankSection = _configuration.GetSection("BankSettings");
            var bankCode = bankSection.GetValue<string>("BankCode");
            var accountNo = bankSection.GetValue<string>("AccountNumber");
            var template = bankSection.GetValue<string>("Template") ?? "compact2";
            var amount = (long)Math.Round(booking.TotalAmount);
            var addInfo = $"BOOKING {booking.Id} CODE {booking.PaymentCode}";

            // VietQR public renderer
            ViewBag.VietQrUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNo}-{template}.png?amount={amount}&addInfo={Uri.EscapeDataString(addInfo)}";

            return View(booking);
        }

        // Tạo mã code thanh toán 6 chữ số
        private string GeneratePaymentCode()
        {
            return _random.Next(100000, 999999).ToString();
        }

        // Hiển thị trang mã code thanh toán
        // VNPay returns here via appsettings ReturnUrl
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            // Use vnp_TxnRef (parsed in library) as our booking id
            int bookingId = 0;
            if (!string.IsNullOrEmpty(response?.OrderId))
            {
                int.TryParse(response.OrderId, out bookingId);
            }

            if (bookingId == 0)
            {
                TempData["PaymentError"] = "Không xác định được đơn thanh toán.";
                return RedirectToAction("MyBookings");
            }

            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                TempData["PaymentError"] = "Đơn đặt phòng không tồn tại.";
                return RedirectToAction("MyBookings");
            }

            if (response.Success && response.VnPayResponseCode == "00")
            {
                booking.Status = "Completed";
                booking.ConfirmationCode = "MLR-" + booking.Id.ToString("D6");
                await _context.SaveChangesAsync();
                TempData["InfoMessage"] = "Thanh toán VNPay thành công. Đặt phòng đã được xác nhận.";
                if (User?.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("MyBookings");
                }
                return RedirectToAction("Confirmation", new { id = booking.Id });
            }

            // failed/cancelled -> keep pending so khách có thể thanh toán lại
            booking.Status = "Payment_Pending";
            await _context.SaveChangesAsync();
            TempData["PaymentError"] = "VNPay chưa thanh toán hoặc giao dịch thất bại. Bạn có thể thanh toán lại.";
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("MyBookings");
            }
            return RedirectToAction("Confirmation", new { id = booking.Id });
        }

        // Người dùng khai báo đã chuyển khoản -> chờ admin xác nhận
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null || booking.Status != "Payment_Pending")
            {
                return NotFound();
            }

            // Đánh dấu là đang chờ admin xác nhận đối soát
            booking.Status = "Awaiting_Admin";
            await _context.SaveChangesAsync();

            TempData["InfoMessage"] = "Bạn đã báo đã thanh toán. Vui lòng đợi quản trị viên xác nhận.";
            return RedirectToAction("Confirmation", new { id = booking.Id });
        }

        // Hủy thanh toán
        [HttpPost]
        public async Task<IActionResult> CancelPayment(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null || booking.Status != "Payment_Pending")
            {
                return NotFound();
            }

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["InfoMessage"] = "Đã hủy thanh toán. Đặt phòng đã bị hủy.";
            return RedirectToAction("Index", "Home");
        }

        // Hoàn tất đặt phòng
        [HttpPost]
        public async Task<IActionResult> CompleteBooking(int id)
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

            // Kiểm tra quyền truy cập
            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
            {
                return Forbid();
            }

            // Chỉ cho phép hoàn tất nếu booking đang ở trạng thái Confirmed
            if (booking.Status != "Confirmed")
            {
                TempData["ErrorMessage"] = "Không thể hoàn tất đặt phòng. Vui lòng kiểm tra trạng thái đặt phòng.";
                return RedirectToAction("Confirmation", new { id = booking.Id });
            }

            // Cập nhật trạng thái thành Completed
            booking.Status = "Completed";
            booking.ConfirmationCode = "MLR-" + booking.Id.ToString("D6");
            await _context.SaveChangesAsync();

            // Chuyển đến trang thông báo thành công
            return RedirectToAction("Success", new { id = booking.Id });
        }

        // Trang thông báo đặt phòng thành công
        [HttpGet]
        public async Task<IActionResult> Success(int id)
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

            // Kiểm tra quyền truy cập
            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
            {
                return Forbid();
            }

            // Only show success page when booking is actually completed
            if (booking.Status != "Completed")
            {
                return RedirectToAction("Confirmation", new { id = booking.Id });
            }

            return View(booking);
        }

        // Danh sách đặt phòng của người dùng hiện tại
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var bookings = await _context.Bookings
                .Where(b => b.UserId == user.Id)
                .Include(b => b.RoomInstance)
                    .ThenInclude(ri => ri.Room)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            // Pass payment pending timeout to view for countdown display
            ViewBag.PendingTimeoutMinutes = _configuration.GetValue<int>("Payment:PendingTimeoutMinutes", 30);

            return View(bookings);
        }

        // Người dùng hủy đặt phòng (khi chưa completed và chưa quá hạn)
        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _context.Bookings
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (user == null || booking == null || booking.UserId != user.Id)
            {
                return NotFound();
            }

            if (booking.Status == "Completed" || booking.Status == "Cancelled")
            {
                return RedirectToAction("MyBookings");
            }

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();
            TempData["InfoMessage"] = "Bạn đã hủy đặt phòng này.";
            return RedirectToAction("MyBookings");
        }

        
    }
}