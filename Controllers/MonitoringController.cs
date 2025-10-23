using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class MonitoringController : Controller
{
    private readonly ILogger<MonitoringController> _logger;

    public MonitoringController(ILogger<MonitoringController> logger)
    {
        _logger = logger;
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Gallon History Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult GallonHistory()
    {
        return View("~/Views/Monitoring/GallonHistory/Index.cshtml");
    }

    public IActionResult GallonActionHistory()
    {
        return View("~/Views/Monitoring/GallonHistory/ActionHistory.cshtml");
    }

    public IActionResult GallonActionReturn()
    {
        return View("~/Views/Monitoring/GallonHistory/ActionReturn.cshtml");
    }



    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// POS Transaction Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult POSTransaction()
    {
        return View("~/Views/Monitoring/POSTransaction/Index.cshtml");
    }

    public IActionResult POSTransactionActionHistory()
    {
        return View("~/Views/Monitoring/POSTransaction/ActionHistory.cshtml");
    }




    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// Credit / Balanced Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult Credit()
    {
        return View("~/Views/Monitoring/Credit/Index.cshtml");
    }

    public IActionResult CreditActionPay()
    {
        return View("~/Views/Monitoring/Credit/ActionPay.cshtml");
    }

    public IActionResult CreditActionNote()
    {
        return View("~/Views/Monitoring/Credit/ActionNote.cshtml");
    }




    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// Top Customer Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult TopCustomer()
    {
        return View("~/Views/Monitoring/TopCustomer/Index.cshtml");
    }





    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// Need Deliver Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult NeedDeliver()
    {
        return View("~/Views/Monitoring/NeedDeliver/Index.cshtml");
    }


    

 
}
