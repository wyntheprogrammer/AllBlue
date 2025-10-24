using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class CustomerController : Controller
{
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ILogger<CustomerController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Add Customer Modal ////////////////////////////////// 
    ////////////////////////////////////////////////////////////////////////////////////////
    public IActionResult AddCustomer()
    {
        return View();
    }

}
