using DinkToPdf.Contracts;
using DinkToPdf;
using Inventory.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Konfigurasi koneksi ke database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"))
);

// Menambahkan layanan autentikasi dan otorisasi dengan cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Halaman login
        options.AccessDeniedPath = "/Auth/Login"; // Halaman akses ditolak
    });

// Menambahkan sesi
builder.Services.AddSession();

// Menambahkan layanan PDF untuk DinkToPdf
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Menambahkan kebijakan otorisasi berdasarkan klaim 'superadmin'
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("superadmin", policy =>
        policy.RequireClaim(ClaimTypes.Role, "superadmin"));

    options.AddPolicy("superadminOrAdmin", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("superadmin") || context.User.IsInRole("admin")
    ));
});

// Menyiapkan aplikasi
var app = builder.Build();

// Mengonfigurasi middleware dalam pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Menggunakan HSTS (HTTP Strict Transport Security)
}

app.UseHttpsRedirection(); // Memaksa HTTPS
app.UseRouting(); // Menggunakan routing untuk controller
app.UseStaticFiles(); // Menggunakan file statis (seperti CSS, JS, dll.)

// Middleware untuk sesi
app.UseSession();

// Middleware untuk otentikasi dan otorisasi
app.UseAuthentication(); // Mengaktifkan autentikasi
app.UseAuthorization();  // Mengaktifkan otorisasi

// Middleware untuk memastikan hanya user yang terautentikasi yang bisa mengakses halaman lain
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // Lewatkan halaman login, logout, dan akses statis
    if (!path.Contains("/assets/") &&
        !path.Contains("/Auth/Login") &&
        !path.Contains("/Auth/Logout") &&
        !context.User.Identity.IsAuthenticated)
    {
        context.Response.Redirect("/Auth/Login");
        return;
    }

    await next.Invoke();
});

// Routing untuk controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets(); // Mendukung pengelolaan aset statis seperti gambar, js, dll.

app.Run(); // Menjalankan aplikasi
