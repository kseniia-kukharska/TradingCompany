using Microsoft.AspNetCore.Authentication.Cookies;
using TradingCompanyBL.Concrete;
using TradingCompanyBL.Interfaces;
using TradingCompanyDal.Concrete;
using TradingCompanyDal.Interfaces;
using TradingCompanyWeb.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Додавання сервісів MVC
builder.Services.AddControllersWithViews();

// Отримання рядка підключення
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                          ?? "Data Source=localhost;Initial Catalog=Software;Integrated Security=True;TrustServerCertificate=True";

// Реєстрація DAL (Інверсія залежностей)
builder.Services.AddTransient<IUserDal>(p => new UserDal(connectionString));
builder.Services.AddTransient<IOrderDal>(p => new OrderDal(connectionString));
builder.Services.AddTransient<IStatusDal>(p => new StatusDal(connectionString));
builder.Services.AddTransient<IProductDal>(p => new ProductDal(connectionString));

// Реєстрація Business Logic
builder.Services.AddTransient<IAuthManager, AuthManager>();
builder.Services.AddTransient<IOrderManager, OrderManager>();

// ВИПРАВЛЕНО: Реєстрація AutoMapper через конфігурацію профілю
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// Налаштування стандартної Cookie-аутентифікації
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.Run();