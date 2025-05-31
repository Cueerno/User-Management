using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using task_4.Data;
using task_4.Models;
using Microsoft.EntityFrameworkCore;


namespace task_4.Controllers;

[Authorize]
public class UserController(ApplicationDbContext context) : Controller {
    
    [Authorize]
    public async Task<IActionResult> All()
    {
        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUser = await context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

        if (currentUser == null || currentUser.IsBlocked)
        {
            return RedirectToAction("Login", "Account"); // Перенаправление заблокированного пользователя
        }

        var users = await context.Users.OrderByDescending(u => u.LastLogin).ToListAsync();
        return View(users);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MassAction([FromBody] MassActionViewModel? model) {
        if (model == null || model.UserIds.Count == 0) {
            return Json(new { message = "No users selected." });
        }

        var users = await context.Users.Where(u => model.UserIds.Contains(u.Id)).ToListAsync();

        switch (model.Action.ToLower()) {
            case "block":
                users.ForEach(u => u.IsBlocked = true);
                break;
            case "unblock":
                users.ForEach(u => u.IsBlocked = false);
                break;
            case "delete":
                context.RemoveRange(users);
                break;
            default:
                return Json(new { message = "Invalid action." });
        }

        await context.SaveChangesAsync();

        var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentUser = await context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

        if (currentUser == null || currentUser.IsBlocked) {
            return Json(new { message = "Your account is affected. Redirecting to home.", redirectToHome = true });
        }

        return Json(new { message = "Action completed successfully." });
    }
}