using Inventory.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Rute login
        options.AccessDeniedPath = "/Auth/Login"; // Rute akses ditolak
    });


// Tambahkan layanan session
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

// Tambahkan middleware session sebelum routing
app.UseSession();

app.UseAuthorization();
app.UseAuthentication();

// Middleware autentikasi sebelum routing
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

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
