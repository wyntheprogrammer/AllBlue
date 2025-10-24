using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class ItemListController : Controller
{
    private readonly ILogger<ItemListController> _logger;

    public ItemListController(ILogger<ItemListController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AddItem()
    {
        return View();
    }

    public IActionResult EditItem()
    {
        return View();
    }


}
