using LuxuryResort.Areas.Identity.Data;
using LuxuryResort.Models;
using Microsoft.AspNetCore.Identity;


namespace LuxuryResort.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<LuxuryResortUser>>();

            // Sửa lại danh sách vai trò
            string[] roleNames = { "Quản lý", "Nhân viên" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Xóa vai trò "Admin" cũ nếu có (chỉ chạy 1 lần)
                    var oldAdminRole = await roleManager.FindByNameAsync("Admin");
                    if (oldAdminRole != null)
                    {
                        await roleManager.DeleteAsync(oldAdminRole);
                    }

                    // Tạo vai trò mới
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Tạo tài khoản Admin mẫu với vai trò "Quản lý"
            var adminUser = await userManager.FindByEmailAsync("admin@manchester.com");
            if (adminUser != null)
            {
                // Đảm bảo user này có vai trò "Quản lý"
                if (!await userManager.IsInRoleAsync(adminUser, "Quản lý"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Quản lý");
                    // Xóa vai trò "Admin" cũ khỏi user
                    await userManager.RemoveFromRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                // Nếu chưa có user admin thì tạo mới
                adminUser = new LuxuryResortUser
                {
                    UserName = "admin@manchester.com",
                    Email = "admin@manchester.com",
                    FullName = "Quản lý Cấp Cao",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Quản lý");
            }

            await SeedRooms(serviceProvider);
            await SeedRoomInstances(serviceProvider);
        }

        private static async Task SeedRooms(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<LuxuryResortContext>();

            // Chỉ thêm dữ liệu nếu bảng Rooms trống
            if (context.Rooms.Any())
            {
                return; // DB has been seeded
            }
            context.Rooms.AddRange(
        new Room
        {
            Type = "Superior Garden View",
            Description = "Phòng tiêu chuẩn, hướng vườn, mang lại cảm giác ấm cúng và gần gũi thiên nhiên.",
            ImageUrl = "https://images.pexels.com/photos/271649/pexels-photo-271649.jpeg",
            PricePerNight = 3200000,
            MaxOccupancy = 2,
            MaxChildren = 1,
            Area = 40,
            BedType = "1 Giường Queen",
            ViewType = "Hướng vườn"
        },
        new Room
        {
            Type = "Deluxe City View",
            Description = "Ngắm nhìn toàn cảnh thành phố sôi động về đêm từ cửa sổ panorama.",
            ImageUrl = "https://images.pexels.com/photos/271639/pexels-photo-271639.jpeg",
            PricePerNight = 4100000,
            MaxOccupancy = 3,
            MaxChildren = 1,
            Area = 45,
            BedType = "1 Giường Queen",
            ViewType = "Hướng thành phố"
        },
        new Room
        {
            Type = "Deluxe Ocean View",
            Description = "Thức dậy với khung cảnh đại dương xanh biếc trải dài đến tận chân trời từ ban công riêng của bạn.",
            ImageUrl = "https://images.pexels.com/photos/1457842/pexels-photo-1457842.jpeg",
            PricePerNight = 5500000,
            MaxOccupancy = 3,
            MaxChildren = 1,
            Area = 55,
            BedType = "1 Giường King",
            ViewType = "Hướng biển"
        },
        new Room
        {
            Type = "Garden View Bungalow",
            Description = "Một không gian nghỉ dưỡng biệt lập, ẩn mình giữa những khu vườn nhiệt đới xanh mướt.",
            ImageUrl = "https://images.pexels.com/photos/2034335/pexels-photo-2034335.jpeg",
            PricePerNight = 7800000,
            MaxOccupancy = 4,
            MaxChildren = 2,
            Area = 120,
            BedType = "1 Giường Super King",
            ViewType = "Hướng vườn"
        },
        new Room
        {
            Type = "Private Pool Villa",
            Description = "Tận hưởng một không gian hoàn toàn riêng tư và đẳng cấp tại Biệt thự hồ bơi của chúng tôi.",
            ImageUrl = "https://images.pexels.com/photos/261169/pexels-photo-261169.jpeg",
            PricePerNight = 12500000,
            MaxOccupancy = 4,
            MaxChildren = 2,
            Area = 120,
            BedType = "1 Giường Super King",
            ViewType = "Hồ bơi riêng"
        },
        new Room
        {
            Type = "Presidential Suite",
            Description = "Đỉnh cao của sự sang trọng với phòng khách, phòng ăn riêng và tầm nhìn panorama toàn cảnh.",
            ImageUrl = "https://images.pexels.com/photos/164595/pexels-photo-164595.jpeg",
            PricePerNight = 55000000,
            MaxOccupancy = 6,
            MaxChildren = 2,
            Area = 250,
            BedType = "2 Giường King",
            ViewType = "Toàn cảnh"
        },
        new Room
        {
            Type = "Oceanfront Residence Villa",
            Description = "Tuyệt tác nghỉ dưỡng 2 phòng ngủ ngay trước biển, với hồ bơi vô cực riêng và quản gia 24/7.",
            ImageUrl = "https://images.pexels.com/photos/221457/pexels-photo-221457.jpeg",
            PricePerNight = 25000000,
            MaxOccupancy = 4,
            MaxChildren = 2,
            Area = 300,
            BedType = "2 Giường King",
            ViewType = "Sát biển"
        }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedRoomInstances(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<LuxuryResortContext>();

            // Chỉ thêm dữ liệu nếu bảng RoomInstances trống
            if (context.RoomInstances.Any())
            {
                return; // DB has been seeded
            }

            // Lấy danh sách Room types
            var rooms = context.Rooms.ToList();
            if (!rooms.Any()) return;

            var roomInstances = new List<LuxuryResort.Areas.Admin.Models.RoomInstance>();

            // Tạo phòng cho mỗi loại
            foreach (var room in rooms)
            {
                // Superior Garden View - 5 phòng
                if (room.Type == "Superior Garden View")
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        roomInstances.Add(new LuxuryResort.Areas.Admin.Models.RoomInstance
                        {
                            RoomNumber = $"10{i}",
                            RoomId = room.Id,
                            Status = "Available"
                        });
                    }
                }
                // Deluxe City View - 4 phòng
                else if (room.Type == "Deluxe City View")
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        roomInstances.Add(new LuxuryResort.Areas.Admin.Models.RoomInstance
                        {
                            RoomNumber = $"20{i}",
                            RoomId = room.Id,
                            Status = "Available"
                        });
                    }
                }
                // Deluxe Ocean View - 3 phòng
                else if (room.Type == "Deluxe Ocean View")
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        roomInstances.Add(new LuxuryResort.Areas.Admin.Models.RoomInstance
                        {
                            RoomNumber = $"30{i}",
                            RoomId = room.Id,
                            Status = "Available"
                        });
                    }
                }
                // Garden View Bungalow - 2 phòng
                else if (room.Type == "Garden View Bungalow")
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        roomInstances.Add(new LuxuryResort.Areas.Admin.Models.RoomInstance
                        {
                            RoomNumber = $"B{i}",
                            RoomId = room.Id,
                            Status = "Available"
                        });
                    }
                }
                // Private Pool Villa - 2 phòng
                else if (room.Type == "Private Pool Villa")
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        roomInstances.Add(new LuxuryResort.Areas.Admin.Models.RoomInstance
                        {
                            RoomNumber = $"V{i}",
                            RoomId = room.Id,
                            Status = "Available"
                        });
                    }
                }
                // Presidential Suite - 1 phòng
                else if (room.Type == "Presidential Suite")
                {
                    roomInstances.Add(new LuxuryResort.Areas.Admin.Models.RoomInstance
                    {
                        RoomNumber = "PS1",
                        RoomId = room.Id,
                        Status = "Available"
                    });
                }
                // Oceanfront Residence Villa - 1 phòng
                else if (room.Type == "Oceanfront Residence Villa")
                {
                    roomInstances.Add(new LuxuryResort.Areas.Admin.Models.RoomInstance
                    {
                        RoomNumber = "OR1",
                        RoomId = room.Id,
                        Status = "Available"
                    });
                }
            }

            context.RoomInstances.AddRange(roomInstances);
            await context.SaveChangesAsync();
        }
    }
}