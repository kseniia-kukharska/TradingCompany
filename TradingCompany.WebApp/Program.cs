using Microsoft.AspNetCore.Authentication.Cookies;
using TradingCompanyBL.Concrete;
using TradingCompanyBL.Interfaces;
using TradingCompanyDal.Concrete;
using TradingCompanyDal.Interfaces;
using TradingCompanyWeb.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                          ?? "Data Source=localhost;Initial Catalog=Software;Integrated Security=True;TrustServerCertificate=True";


builder.Services.AddTransient<IUserDal>(p => new UserDal(connectionString));
builder.Services.AddTransient<IOrderDal>(p => new OrderDal(connectionString));
builder.Services.AddTransient<IStatusDal>(p => new StatusDal(connectionString));
builder.Services.AddTransient<IProductDal>(p => new ProductDal(connectionString));

builder.Services.AddTransient<IAuthManager, AuthManager>();
builder.Services.AddTransient<IOrderManager, OrderManager>();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

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