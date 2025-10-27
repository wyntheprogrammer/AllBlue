using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class SystemSettingsController : Controller
{
    private readonly ILogger<SystemSettingsController> _logger;

    public SystemSettingsController(ILogger<SystemSettingsController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

}
