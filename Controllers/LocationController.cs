using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class LocationController : Controller
{
    private readonly ILogger<LocationController> _logger;

    public LocationController(ILogger<LocationController> logger)
    {
        _logger = logger;
    }


    public IActionResult LocationCity()
    {
        return View("~/Views/Location/City/Index.cshtml");
    }




    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// POS Transaction Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////
    public IActionResult LocationBarangay()
    {
        return View("~/Views/Location/Barangay/Index.cshtml");
    }

    public IActionResult EditBarangay()
    {
        return View("~/Views/Location/Barangay/EditBarangay.cshtml");
    }

}
