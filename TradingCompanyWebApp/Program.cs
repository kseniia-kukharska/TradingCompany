using Microsoft.AspNetCore.Authentication.Cookies;
using TradingCompanyBL.Concrete;
using TradingCompanyBL.Interfaces;
using TradingCompanyDal.Concrete;
using TradingCompanyDal.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Отримання рядка підключення для SQL Server
string connString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Реєстрація сервісів (DI)
builder.Services.AddTransient<IOrderDal>(p => new OrderDal(connString));
builder.Services.AddTransient<IStatusDal>(p => new StatusDal(connString));
builder.Services.AddTransient<IUserDal>(p => new UserDal(connString));
builder.Services.AddTransient<IOrderManager, OrderManager>();
builder.Services.AddTransient<IAuthManager, AuthManager>();

// 3. AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// 4. Налаштування Cookies та маршрутів для авторизації
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Порядок важливий: спосіб розпізнавання (Authn) -> перевірка прав (Authz)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();