
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using SysClaim = System.Security.Claims.Claim;
using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Services;

namespace CMCS.Controllers;
public class AccountController : Controller
{
    private readonly IUserService _users;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService users, ILogger<AccountController> logger)
    {
        _users = users;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserDto model, string? returnUrl = null)
    {
       
        _logger.LogInformation("Login POST received. Username: {Username}", model?.Username ?? "(null)");
        _logger.LogInformation("Request.Form keys: {Keys}", string.Join(", ", Request.Form.Keys));
        foreach (var k in Request.Form.Keys)
        {
            _logger.LogInformation("Form[{Key}] = {Val}", k, Request.Form[k]);
        }
        if (model == null)
        {
            ModelState.AddModelError("", "Invalid form submission.");
            TempData["Error"] = "Invalid form submission (model binding failed).";
            return View();
        }

        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError("", "Username and Password are required.");
            TempData["Error"] = "Please provide both username and password.";
            return View(model);
        }

        var user = _users.ValidateCredentials(model.Username, model.Password);
        if (user == null)
        {
            _logger.LogWarning("Login failed for user {Username}", model.Username);
            ModelState.AddModelError("", "Invalid username or password.");
            TempData["Error"] = "Invalid username or password.";
            return View(model);
        }

        _logger.LogInformation("Login successful for {Username} with role {Role}", user.Username, user.Role);

        var claims = new List<SysClaim>
        {
            new SysClaim(ClaimTypes.Name, user.Username),
            new SysClaim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        // Redirect by role
        if (user.Role == "Lecturer") return RedirectToAction("MyClaims", "Claim");
        if (user.Role == "Coordinator" || user.Role == "Manager") return RedirectToAction("Index", "Admin");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}
