using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class CustomerController : Controller
{
    private readonly ILogger<CustomerController> _logger;
    private readonly AppDbContext _context;

    public CustomerController(ILogger<CustomerController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
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
