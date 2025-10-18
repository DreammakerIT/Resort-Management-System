using LuxuryResort.Areas.Identity.Data;
using LuxuryResort.Data;
using LuxuryResort.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuxuryResort.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<LuxuryResortUser> _userManager;
        private readonly LuxuryResortContext _context;

        public ProfileController(UserManager<LuxuryResortUser> userManager, LuxuryResortContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy lịch sử đặt phòng của khách hàng
            var bookings = await _context.Bookings
                .Include(b => b.RoomInstance)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.CheckInDate)
                .ToListAsync();

            // Lấy thống kê đơn giản
            var totalBookings = bookings.Count;
            var totalNights = bookings.Sum(b => (b.CheckOutDate - b.CheckInDate).Days);
            var totalSpent = bookings.Sum(b => b.TotalAmount);

            var profileViewModel = new ProfileViewModel
            {
                User = user,
                Bookings = bookings,
                TotalBookings = totalBookings,
                TotalNights = totalNights,
                TotalSpent = totalSpent
            };

            return View(profileViewModel);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var editViewModel = new ProfileEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Nationality = user.Nationality,
                PassportNumber = user.PassportNumber
            };

            return View(editViewModel);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Address = model.Address;
            user.Nationality = model.Nationality;
            user.PassportNumber = model.PassportNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Thông tin cá nhân đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Profile/BookingHistory
        public async Task<IActionResult> BookingHistory()
        {
            var userId = _userManager.GetUserId(User);
            var allBookings = await _context.Bookings
                                            .Include(b => b.RoomInstance).ThenInclude(ri => ri.Room)
                                            .Where(b => b.UserId == userId)
                                            .OrderByDescending(b => b.CheckInDate) // Sắp xếp theo ngày mới nhất
                                            .ToListAsync();

            // Trả về trực tiếp danh sách, không cần ViewModel
            return View(allBookings);
        }

        // GET: Profile/Preferences
        public async Task<IActionResult> Preferences()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var preferencesViewModel = new PreferencesViewModel
            {
                UserId = user.Id,
                PreferredRoomType = user.PreferredRoomType,
                SpecialRequests = user.SpecialRequests,
                MarketingConsent = user.MarketingConsent,
                NewsletterSubscription = user.NewsletterSubscription
            };

            return View(preferencesViewModel);
        }

        // POST: Profile/Preferences
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Preferences(PreferencesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật preferences
            user.PreferredRoomType = model.PreferredRoomType;
            user.SpecialRequests = model.SpecialRequests;
            user.MarketingConsent = model.MarketingConsent;
            user.NewsletterSubscription = model.NewsletterSubscription;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Tùy chọn cá nhân đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
