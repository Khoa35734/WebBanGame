using WebBanGame.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Thêm services vào container
builder.Services.AddControllersWithViews();

// Cấu hình DbContext
builder.Services.AddDbContext<BanGameBanQuyenContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("BanGameBanQuyenConnect")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/DangNhap"; // Trang đăng nhập
		options.AccessDeniedPath = "/Account/AccessDenied"; // Trang từ chối truy cập
	});
var app = builder.Build();

// Cấu hình middleware
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Cấu hình routing với Area và Default
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
//pattern: "{controller=Account}/{action=DangNhap}/{id?}");

app.Run();
