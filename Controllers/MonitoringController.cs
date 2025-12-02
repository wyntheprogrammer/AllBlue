using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllBlue.Controllers;

public class MonitoringController : Controller
{
    private readonly ILogger<MonitoringController> _logger;
    private readonly AppDbContext _context;

    public MonitoringController(ILogger<MonitoringController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
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

    public IActionResult POSTransaction(int id, int page = 1, int pageSize = 5, int window = 1, string search = "", int? selectedBarangayID = null, string dateFrom = "", string dateTo = "")
    {
        var orders = _context.Order
            .Include(o => o.customer)
                .ThenInclude(c => c.barangay)
            .Include(o => o.payment)
            .ToList()
            .GroupBy(o => o.Payment_ID)
            .Select(g => {
                var cust = g.First().customer;
                var pay = g.First().payment;
                var barangay = cust?.barangay;

                return new OrderDisplayItem
                {
                    OrderID = g.Key,

                    // NULL SAFE Full Name
                    CustomerName = string.Join(" ",
                        new[]
                        {
                            cust?.First_Name,
                            cust?.Last_Name
                        }.Where(x => !string.IsNullOrWhiteSpace(x))
                    ),

                    // NULL SAFE Address
                    CustomerAddress = string.Join(", ",
                        new[]
                        {
                            cust?.Street,
                            barangay?.Name
                        }.Where(x => !string.IsNullOrWhiteSpace(x))
                    ),

                    Date = pay.Date,
                    Free = pay.Free,
                    Quantity = pay.Quantity ?? 0,
                    Balanced = pay.Balanced ?? 0,
                    Service = pay.Service,
                    Status = pay.Status,
                    Total = pay.Total,
                    Barangay_ID = cust?.Barangay_ID ?? 0
                };
            })
            .ToList();


            // -------------------------------
            // Search Filter
            // -------------------------------

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalized = search.Trim().ToLower();

                orders = orders
                    .Where(o =>
                        o.CustomerName.ToLower().Contains(normalized) ||
                        o.CustomerAddress.ToLower().Contains(normalized))
                    .ToList();
            }

            // -------------------------------
            // 📍 Barangay Filter
            // -------------------------------

            if (selectedBarangayID.HasValue)
            {
                orders = orders
                    .Where(o => o.Barangay_ID == selectedBarangayID.Value)
                    .ToList();
            }


            // -------------------------------
            // 📅 Date From / Date To Filter
            // -------------------------------
            if (!string.IsNullOrWhiteSpace(dateFrom) || !string.IsNullOrWhiteSpace(dateTo))
            {
                DateTime fromDateTime, toDateTime;
                DateOnly fromDateOnly, toDateOnly;

                bool hasFrom = DateTime.TryParse(dateFrom, out fromDateTime);
                bool hasTo = DateTime.TryParse(dateTo, out toDateTime);

                if (hasFrom)
                {
                    fromDateOnly = DateOnly.FromDateTime(fromDateTime);
                    orders = orders.Where(o => o.Date >= fromDateOnly).ToList();
                }

                if (hasTo)
                {
                    toDateOnly = DateOnly.FromDateTime(toDateTime);
                    orders = orders.Where(o => o.Date <= toDateOnly).ToList();
                }
            }



            // Pagination
            int totalOrder = orders.Count();
            int totalPages = (int)Math.Ceiling((double)totalOrder / pageSize);

            int windowSize = 5;
            int startPage = ((window - 1) * windowSize) + 1;
            int endPage = Math.Min(startPage + windowSize - 1, totalPages);

            var pagedOrder = orders
                .OrderBy(o => o.OrderID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.Window = window;
            ViewBag.Search = search;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;



            // Barangay List
            var barangay = _context.Barangay.ToList();

            ViewBag.barangayList = new SelectList(
                barangay.Select(b => new SelectListItem
                {
                    Value = b.Barangay_ID.ToString(),
                    Text = b.Name
                }).ToList(),
                "Value",
                "Text"
            );


        return View("~/Views/Monitoring/POSTransaction/Index.cshtml", pagedOrder);
    }

    [HttpGet]
    public IActionResult POSTransactionActionHistory(int id)
    {
        var order = _context.Order.FirstOrDefault(o => o.Order_ID == id);
        if (order == null)
        {
            return NotFound();
        }

        return PartialView("~/Views/Monitoring/POSTransaction/ActionHistory.cshtml", order);
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
