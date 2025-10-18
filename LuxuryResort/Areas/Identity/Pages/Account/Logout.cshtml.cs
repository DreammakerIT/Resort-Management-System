// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LuxuryResort.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace LuxuryResort.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<LuxuryResortUser> _signInManager;
        // Thêm UserManager vào
        private readonly UserManager<LuxuryResortUser> _userManager;
        private readonly ILogger<LogoutModel> _logger;

        // Cập nhật constructor để nhận UserManager
        public LogoutModel(SignInManager<LuxuryResortUser> signInManager, UserManager<LuxuryResortUser> userManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager; // Gán giá trị
            _logger = logger;
        }

        // Phương thức OnPost sẽ được gọi khi người dùng bấm nút Logout
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            string logoutRedirectUrl;

            // Lấy thông tin người dùng hiện tại TRƯỚC KHI đăng xuất
            var user = await _userManager.GetUserAsync(User);

            // Kiểm tra user có tồn tại và có phải là Admin không
            // Kiểm tra các role quản trị viên
            if (user != null && (await _userManager.IsInRoleAsync(user, "Quản lý") || await _userManager.IsInRoleAsync(user, "Nhân viên")))
            {
                // Nếu là Admin, chỉ định trang trả về là trang đăng nhập với ReturnUrl
                logoutRedirectUrl = "/Identity/Account/Login?ReturnUrl=%2FAdmin";
            }
            else
            {
                // Nếu là người dùng thường, trả về trang chủ hoặc returnUrl
                logoutRedirectUrl = returnUrl ?? "/";
            }

            // Thực hiện đăng xuất
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // Điều hướng đến trang đã được quyết định ở trên
            return LocalRedirect(logoutRedirectUrl);
        }
    }
}
