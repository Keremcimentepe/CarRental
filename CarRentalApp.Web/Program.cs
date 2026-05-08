


using Microsoft.AspNetCore.Authentication.Cookies;
using CarRentalApp.Business.Services;
using CarRentalApp.DataAccess.Repositories;
using CarRentalApp.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Sunum katmanı için MVC (Model-View-Controller) servisini ekliyoruz
builder.Services.AddControllersWithViews();
// Oturum yönetimi ve kimlik doğrulama ayarları
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Giriş yapılmamışsa yönlendirilecek sayfa
        options.AccessDeniedPath = "/Home/AccessDenied"; // Yetkisiz girişte yönlendirilecek sayfa
    });

// Veritabanı bağlamını (DbContext) ve PostgreSQL bağlantısını ayarlıyoruz
builder.Services.AddDbContext<AppDbContext>(options =>

    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Repository pattern'i sisteme tanıtıyoruz
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRentalService, RentalService>();

var app = builder.Build();

// Hata ayıklama ve yönlendirme ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Güvenlik için
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Statik dosyaların (CSS, JS, Resimler) kullanımını açıyoruz
app.MapStaticAssets(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();