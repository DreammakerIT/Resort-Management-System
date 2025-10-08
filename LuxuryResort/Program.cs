using LuxuryResort.Areas.Identity.Data;
using LuxuryResort.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LuxuryResortContextConnection") ?? throw new InvalidOperationException("Connection string 'LuxuryResortContextConnection' not found.");

builder.Services.AddDbContext<LuxuryResortContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<LuxuryResortUser, IdentityRole>() 
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<LuxuryResortContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<BankSettings>(builder.Configuration.GetSection("BankSettings"));
builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session tồn tại
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

//?O?N CODE NÀY ?? T? ??NG T?O TÀI KHO?N ADMIN
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
