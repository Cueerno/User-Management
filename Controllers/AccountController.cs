using System.Security.Claims;
using task_4.Data;
using task_4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace task_4.Controllers;

public class AccountController(ApplicationDbContext context) : Controller {
    // GET: /Account/Register
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model) {
        if (!ModelState.IsValid) {
            return View(model);
        }

        var user = new User {
            Name = model.Name,
            Email = model.Email,
            Password = HashPassword(model.Password),
            CreatedAt = DateTimeOffset.UtcNow,
            LastLogin = DateTimeOffset.UtcNow,
            IsBlocked = false
        };

        context.Users.Add(user);

        try {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException) {
            ModelState.AddModelError("Email", "Email already exists.");
            return View(model);
        }

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Login
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model) {
        if (!ModelState.IsValid) {
            return View(model);
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null || user.Password != HashPassword(model.Password)) {
            ModelState.AddModelError("", "Bad credentials.");
            return View(model);
        }
        
        user.LastLogin = DateTimeOffset.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync();

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    private static string HashPassword(string password) {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}