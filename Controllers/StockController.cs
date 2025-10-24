using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class StockController : Controller
{
    private readonly ILogger<StockController> _logger;

    public StockController(ILogger<StockController> logger)
    {
        _logger = logger;
    }


    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Stock Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult Stock()
    {
        return View("~/Views/Stock/Stock/Index.cshtml");
    }

    public IActionResult StockIn()
    {
        return View("~/Views/Stock/Stock/StockIn.cshtml");
    }


    public IActionResult StockOut()
    {
        return View("~/Views/Stock/Stock/StockOut.cshtml");
    }
    
    public IActionResult StockHistory()
    {
        return View("~/Views/Stock/Stock/StockHistory.cshtml");
    }
    

    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Expenses Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult Expenses()
    {
        return View("~/Views/Stock/Expenses/Index.cshtml");
    }

    public IActionResult AddExpenses()
    {
        return View("~/Views/Stock/Expenses/AddExpenses.cshtml");
    }

    public IActionResult ExpensesHistory()
    {
        return View("~/Views/Stock/Expenses/ExpensesHistory.cshtml");
    }



}
