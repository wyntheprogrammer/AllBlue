using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class AccountTypeController : Controller
{
    private readonly ILogger<AccountTypeController> _logger;

    public AccountTypeController(ILogger<AccountTypeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult EditAccount()
    {
        return View();
    }
    

}
