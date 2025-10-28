using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllBlue.Controllers;

public class LocationController : Controller
{
    private readonly ILogger<LocationController> _logger;
    private readonly AppDbContext _context;


    public LocationController(ILogger<LocationController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }


    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////// City Module ///////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////
    [HttpGet]
    public IActionResult LocationCity(string search, int page = 1, int pageSize = 5, int window = 1)
    {
        var city = _context.City.AsQueryable();

        if (!string.IsNullOrEmpty(search?.Trim()))
        {
            string keyword = search.Trim().ToLower();
            city = city.Where(c => c.Name.ToLower().Contains(keyword));
        }

        int totalCity = city.Count();
        int totalPages = (int)Math.Ceiling((double)totalCity / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var pagedCity = city
            .OrderBy(c => c.City_ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;


        return View("~/Views/Location/City/Index.cshtml", pagedCity);
    }


    //////////////////////////////// Add City Module ///////////////////////////////////////
    [HttpGet]
    public IActionResult AddCity()
    {
        return View("~/Views/Location/City/AddCity.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> AddCity(City city)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.City.Add(city);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "City added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
           
            return RedirectToAction("LocationCity");
        }
        
        return RedirectToAction("LocationCity"); 
    }



    //////////////////////////////// Edit City Module ///////////////////////////////////////
    [HttpGet]
    public IActionResult EditCity(int id)
    {
        var city = _context.City.FirstOrDefault(c => c.City_ID == id);
        if (city == null)
        {
            return NotFound();
        }

        return PartialView("~/Views/Location/City/EditCity.cshtml", city);
    }

    [HttpPost]
    public IActionResult EditCity(City city)
    {
        var existingCity = _context.City.FirstOrDefault(c => c.City_ID == city.City_ID);
        if (existingCity == null)
        {
            TempData["ErrorMessage"] = "Failed to update city.";
            return NotFound();
        }

        try {
            existingCity.Name = city.Name;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "City updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        
        return RedirectToAction("LocationCity");
    }


    //////////////////////////////// Delete City Module ///////////////////////////////////////
    [HttpGet]
    public IActionResult DeleteCity(int id)
    {
        var city = _context.City.FirstOrDefault(c => c.City_ID == id);
        if (city == null)
        {
            return NotFound();
        }

        return PartialView("~/Views/Location/City/DeleteCity.cshtml", city);
    }

    [HttpPost]
    public IActionResult DeleteCity(City city)
    {
        var existingCity = _context.City.FirstOrDefault(c => c.City_ID == city.City_ID);
        if (existingCity == null)
        {
            TempData["ErrorMessage"] = "City not found.";
            
            return NotFound();
        }

        try
        {
            _context.City.Remove(existingCity);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "City deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        
        return RedirectToAction("LocationCity");
    
    }


    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// Barangay Module ///////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////
    public IActionResult LocationBarangay(string search, int page = 1, int pageSize = 5, int window = 1)
    {
        var barangay = _context.Barangay.AsQueryable();

        if (!string.IsNullOrEmpty(search?.Trim()))
        {
            string keyword = search.Trim().ToLower();
            barangay = barangay.Where(b =>
                b.Name.ToLower().Contains(keyword) ||
                b.city.Name.ToLower().Contains(keyword));
        }

        int totalBarangay = barangay.Count();
        int totalPages = (int)Math.Ceiling((double)totalBarangay / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var pagedBarangay = barangay
            .OrderBy(b => b.Barangay_ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(b => b.city)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;

        return View("~/Views/Location/Barangay/Index.cshtml", pagedBarangay);
    }


    //////////////////////////////// Add Barangay Module ///////////////////////////////////////
    [HttpGet]
    public IActionResult AddBarangay()
    {

        var cityList = _context.City.Select(c => new SelectListItem
        {
            Value = c.City_ID.ToString(),
            Text = c.Name
        }).ToList();

        ViewBag.CityList = new SelectList(cityList, "Value", "Text");

        return View("~/Views/Location/Barangay/AddBarangay.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> AddBarangay(Barangay barangay)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Barangay.Add(barangay);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Barangay added successfully!";
               
            } catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            
            return RedirectToAction("LocationBarangay");
        }

        TempData["ErrorMessage"] = string.Join(" | ", ModelState.Select(kvp =>
                $"{kvp.Key}: {string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))}"));

        return RedirectToAction("LocationBarangay"); // or return View(barangay);
    }



    //////////////////////////////// Edit Barangay Module ///////////////////////////////////////
    [HttpGet]
    public IActionResult EditBarangay(int id)
    {
        var barangay = _context.Barangay.FirstOrDefault(b => b.Barangay_ID == id);
        if (barangay == null)
        {
            return NotFound();
        }

        var cityList = _context.City.Select(c => new SelectListItem
        {
            Value = c.City_ID.ToString(),
            Text = c.Name
        }).ToList();


        ViewBag.CityList = new SelectList(cityList, "Value", "Text");

        return PartialView("~/Views/Location/Barangay/EditBarangay.cshtml", barangay);
    }

    [HttpPost]
    public IActionResult EditBarangay(Barangay barangay)
    {
        var existingBarangay = _context.Barangay.FirstOrDefault(b => b.Barangay_ID == barangay.Barangay_ID);
        if (existingBarangay == null)
        {
            TempData["ErrorMessage"] = "Failed to update barangay.";
            return NotFound();
        }

        try
        {
            existingBarangay.Name = barangay.Name;
            existingBarangay.City_ID = barangay.City_ID;
            existingBarangay.Color = barangay.Color;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Barangay updated successfully!";
        } catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        
        return RedirectToAction("LocationBarangay");
    }



    //////////////////////////////// Delete Barangay Module ///////////////////////////////////////
    [HttpGet]
    public IActionResult DeleteBarangay(int id)
    {
        var barangay = _context.Barangay.FirstOrDefault(b => b.Barangay_ID == id);
        if (barangay == null)
        {
            return NotFound();
        }

        return PartialView("~/Views/Location/Barangay/DeleteBarangay.cshtml", barangay);
    }

    [HttpPost]
    public IActionResult DeleteBarangay(Barangay barangay)
    {
        var existingBarangay = _context.Barangay.FirstOrDefault(b => b.Barangay_ID == barangay.Barangay_ID);
        if (existingBarangay == null)
        {
            TempData["ErrorMessage"] = "Failed to delete barangay. Please check the form for errors.";
            return NotFound();
        }

        try
        {
            _context.Barangay.Remove(existingBarangay);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Barangay deleted successfully!";
        } catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        
        return RedirectToAction("LocationBarangay");
    }
}
