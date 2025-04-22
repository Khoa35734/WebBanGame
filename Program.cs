using WebBanGame.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm services vào container
builder.Services.AddControllersWithViews();

// Cấu hình DbContext
builder.Services.AddDbContext<BanGameBanQuyenContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("BanGameBanQuyenConnect")));

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

app.Run();
