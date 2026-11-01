var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ==========================================
// ĐĂNG KÝ CÁC SERVICES (Phải nằm TRƯỚC builder.Build())
// ==========================================
builder.Services.AddScoped<WebNgheNhac.Models.DbHelper>();
builder.Services.AddScoped<WebNgheNhac.Services.SpotifyService>();
builder.Services.AddSession();


// Build ứng dụng (Từ đây trở xuống KHÔNG được thêm Services nữa)
var app = builder.Build();


// ==========================================
// CẤU HÌNH PIPELINE
// ==========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Session phải đặt trước Authorization
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();