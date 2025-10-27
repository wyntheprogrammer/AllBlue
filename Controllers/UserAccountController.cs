using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class UserAccountController : Controller
{
    private readonly ILogger<UserAccountController> _logger;

    public UserAccountController(ILogger<UserAccountController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult EditUser()
    {
        return View();
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

}
