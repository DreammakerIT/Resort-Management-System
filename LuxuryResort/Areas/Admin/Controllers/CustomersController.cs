using LuxuryResort.Areas.Admin.Models;
using LuxuryResort.Areas.Identity.Data;
using LuxuryResort.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuxuryResort.Areas.Admin.Controllers
{
    public class CustomersController : BaseController
    {
        private readonly UserManager<LuxuryResortUser> _userManager;

        public CustomersController(UserManager<LuxuryResortUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Admin/Customers
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách tất cả người dùng có vai trò là "Customer" từ database
            var customersInDb = await _userManager.GetUsersInRoleAsync("Customer");

            // Chuyển đổi dữ liệu sang ViewModel để hiển thị
            var customersViewModel = customersInDb.Select(user => new CustomerViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow
            }).ToList();

            return View(customersViewModel);
        }

        // POST: /Admin/Customers/Lock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Customers/Unlock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            return RedirectToAction(nameof(Index));
        }

        // Thêm vào trong CustomersController.cs

        // GET: /Admin/Customers/Delete/user-id
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /Admin/Customers/Delete/user-id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    // Xử lý lỗi nếu không xóa được
                    throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{id}'.");
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}