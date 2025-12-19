using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingCompanyBL.Interfaces;
using TradingCompanyWeb.Models;

namespace TradingCompanyWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _authManager.Login(model.Username, model.Password);

            if (user != null)
            {
                string role = user.RoleId == 4 ? "Seller" : "Viewer";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("UserId", user.UserId.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                _logger.LogInformation("User {User} logged in as {Role}", model.Username, role);
                return RedirectToAction("Index", "Orders");
            }
            ViewBag.Error = "Invalid login or password";
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access Denied for user {User} at {Time}", User.Identity?.Name, DateTime.Now);
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}