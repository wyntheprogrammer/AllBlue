using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AllBlue.Controllers;

public class SystemSettingsController : Controller
{
    private readonly ILogger<SystemSettingsController> _logger;
    private readonly AppDbContext _context;

    public SystemSettingsController(ILogger<SystemSettingsController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        int id = 1;
        var settings = _context.Settings.FirstOrDefault(s => s.SettingsID == id);
        if (settings == null)
        {
            return NotFound();
        }

        var brgy = _context.Barangay.ToList();

        ViewBag.barangayList = new SelectList(
            brgy.Select(b => new SelectListItem
            {
                Value = b.Barangay_ID.ToString(),
                Text = b.Name
            }),
            "Value",
            "Text"
        );

        var city = _context.City.ToList();

        ViewBag.cityList = new SelectList(
            city.Select(b => new SelectListItem
            {
                Value = b.City_ID.ToString(),
                Text = b.Name
            }),
            "Value",
            "Text"
        );

        return View(settings);
    }

    [HttpPost]
    public IActionResult Index(Settings settings)
    {
        var existingSettings = _context.Settings.FirstOrDefault(s => s.SettingsID == settings.SettingsID);
        if (existingSettings == null)
        {
            TempData["ErrorMessage"] = "Failed to update settings.";
            return NotFound();
        }

        try
        {
            existingSettings.Title = settings.Title;
            existingSettings.Footer = settings.Footer;
            existingSettings.System = settings.System;
            existingSettings.Contact1 = settings.Contact1;
            existingSettings.Contact2 = settings.Contact2;
            existingSettings.Contact3 = settings.Contact3;
            existingSettings.Barangay_ID = settings.Barangay_ID;
            existingSettings.City_ID = settings.City_ID;
            existingSettings.Province = settings.Province;
            existingSettings.ZipCode = settings.ZipCode;
            existingSettings.Country = settings.Country;

            existingSettings.DeliveryInterval = settings.DeliveryInterval;
            existingSettings.LocalAddress = settings.LocalAddress;
            existingSettings.Favicon = settings.Favicon;
            existingSettings.Logo = settings.Logo;
            existingSettings.Banner = settings.Banner;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Settings updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        // 🔥 REBUILD VIEWBAGS (REQUIRED)
        var brgy = _context.Barangay.ToList();
        ViewBag.BarangayList = new SelectList(brgy, "Barangay_ID", "Name");

        var city = _context.City.ToList();
        ViewBag.CityList = new SelectList(city, "City_ID", "Name");

        // 🔥 RETURN MODEL BACK TO VIEW
        return View("Index", existingSettings);
    }



}
