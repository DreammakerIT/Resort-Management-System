using LuxuryResort.Areas.Admin.Models;
using LuxuryResort.Areas.Identity.Data;
using LuxuryResort.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuxuryResort.Areas.Admin.Controllers
{
    // Kế thừa từ BaseController để được bảo vệ
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Quản lý")]
    public class StaffController : BaseController
    {
        private readonly UserManager<LuxuryResortUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StaffController(UserManager<LuxuryResortUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin/Staff
        // Trong file StaffController.cs
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var staffList = new List<StaffViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // SỬA LẠI ĐIỀU KIỆN IF TẠI ĐÂY
                if (roles.Contains("Quản lý") || roles.Contains("Nhân viên"))
                {
                    staffList.Add(new StaffViewModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        EmailConfirmed = user.EmailConfirmed,
                        LockoutEnd = user.LockoutEnd,
                        Roles = roles
                    });
                }
            }
            return View(staffList);
        }

        // GET: /Admin/Staff/Create
        public IActionResult Create()
        {
            // Lấy danh sách vai trò để hiển thị trong form
            ViewBag.Roles = _roleManager.Roles
                                .Where(r => r.Name == "Quản lý" || r.Name == "Nhân viên")  
                                .Select(r => r.Name).ToList();
            return View();
        }

        // POST: /Admin/Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new LuxuryResortUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true // Tự động xác thực email cho tài khoản admin
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Gán vai trò cho user
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Nếu có lỗi, trả lại form với danh sách vai trò
            ViewBag.Roles = _roleManager.Roles.Where(r => r.Name == "Quản Lý" || r.Name == "Nhân viên").Select(r => r.Name).ToList();
            return View(model);
        }

        // Thêm vào trong StaffController.cs

        // GET: /Admin/Staff/Edit/some-id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles
                .Where(r => r.Name == "Quản lý" || r.Name == "Nhân viên")
                .Select(r => r.Name).ToListAsync();

            var model = new EditStaffViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserRoles = userRoles,
                AllRoles = allRoles
            };

            return View(model);
        }

        // POST: /Admin/Staff/Edit/some-id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditStaffViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // Lấy lại danh sách vai trò để hiển thị lại nếu có lỗi
            model.AllRoles = await _roleManager.Roles
                .Where(r => r.Name == "Quản lý" || r.Name == "Nhân viên")
                .Select(r => r.Name).ToListAsync();

            if (ModelState.IsValid)
            {
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }

                // Cập nhật mật khẩu nếu có
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                    if (!removePasswordResult.Succeeded)
                    {
                        foreach (var error in removePasswordResult.Errors) ModelState.AddModelError(string.Empty, error.Description);
                        return View(model);
                    }

                    var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    if (!addPasswordResult.Succeeded)
                    {
                        foreach (var error in addPasswordResult.Errors) ModelState.AddModelError(string.Empty, error.Description);
                        return View(model);
                    }
                }

                // Cập nhật vai trò
                var oldRoles = await _userManager.GetRolesAsync(user);
                var staffRoles = oldRoles.Where(r => r != "Customer"); // Chỉ xóa các vai trò nhân viên, giữ vai trò Customer nếu có
                await _userManager.RemoveFromRolesAsync(user, staffRoles);
                await _userManager.AddToRolesAsync(user, model.UserRoles);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // Thêm vào trong StaffController.cs

        // GET: /Admin/Staff/Delete/some-id
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

        // POST: /Admin/Staff/Delete/some-id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Không cho phép tự xóa tài khoản của chính mình
                var currentUserId = _userManager.GetUserId(User);
                if (id == currentUserId)
                {
                    // Thêm lỗi và trả về trang xác nhận xóa
                    ModelState.AddModelError(string.Empty, "Bạn không thể tự xóa tài khoản của chính mình.");
                    return View("Delete", user);
                }

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