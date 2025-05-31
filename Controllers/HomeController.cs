using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using task_4.Models;

namespace task_4.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller {

    public IActionResult Index() {
        ViewBag.UserName = User.Identity is { IsAuthenticated: true } ? User.Identity.Name : "Guest";
        return View();
    }
    
    public IActionResult Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}