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

    public IActionResult POSTransaction(
    int id,
    int page = 1,
    int pageSize = 5,
    int window = 1,
    string search = "",
    int? selectedBarangayID = null,
    string dateFrom = "",
    string dateTo = "")
    {
        // ------------------------------------------------------
        // 1. LOAD PAYMENTS WITH RELATED DATA
        // ------------------------------------------------------
        var payments = _context.Payment
            .Include(p => p.Orders)
                .ThenInclude(o => o.customer)
                    .ThenInclude(c => c.barangay)
            .Include(p => p.Orders)
                .ThenInclude(o => o.userAccount)
            .ToList()

            // Convert to display items (one per payment)
            .Select(payment =>
            {
                var firstOrder = payment.Orders.FirstOrDefault();
                var cust = firstOrder?.customer;
                var barangay = cust?.barangay;

                return new OrderDisplayItem
                {
                    PaymentID = payment.Payment_ID,

                    CustomerName = cust == null
                        ? ""
                        : $"{cust.First_Name} {cust.Last_Name}".Trim(),

                    CustomerAddress = string.Join(", ",
                        new[]
                        {
                            cust?.Street,
                            barangay?.Name
                        }.Where(x => !string.IsNullOrWhiteSpace(x))
                    ),

                    Date = payment.Date,
                    Free = payment.Free,
                    Quantity = payment.Quantity ?? 0,
                    Balanced = payment.Balanced ?? 0,
                    Service = payment.Service,
                    Status = payment.Status,
                    Total = payment.Total,
                    Barangay_ID = cust?.Barangay_ID ?? 0
                };
            })
            .ToList();

        // ------------------------------------------------------
        // 2. SEARCH FILTER
        // ------------------------------------------------------
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.ToLower().Trim();

            payments = payments
                .Where(p =>
                    p.CustomerName.ToLower().Contains(keyword) ||
                    p.CustomerAddress.ToLower().Contains(keyword))
                .ToList();
        }

        // ------------------------------------------------------
        // 3. BARANGAY FILTER
        // ------------------------------------------------------
        if (selectedBarangayID.HasValue)
        {
            payments = payments
                .Where(p => p.Barangay_ID == selectedBarangayID)
                .ToList();
        }

        // ------------------------------------------------------
        // 4. DATE FILTER
        // ------------------------------------------------------
        if (!string.IsNullOrWhiteSpace(dateFrom))
        {
            if (DateTime.TryParse(dateFrom, out var df))
            {
                var from = DateOnly.FromDateTime(df);
                payments = payments.Where(p => p.Date >= from).ToList();
            }
        }

        if (!string.IsNullOrWhiteSpace(dateTo))
        {
            if (DateTime.TryParse(dateTo, out var dt))
            {
                var to = DateOnly.FromDateTime(dt);
                payments = payments.Where(p => p.Date <= to).ToList();
            }
        }

        // ------------------------------------------------------
        // 5. PAGINATION
        // ------------------------------------------------------
        int total = payments.Count;
        int totalPages = (int)Math.Ceiling((double)total / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var paged = payments
            .OrderBy(p => p.OrderID)
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

        // ------------------------------------------------------
        // 6. Barangay List
        // ------------------------------------------------------
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

        return View("~/Views/Monitoring/POSTransaction/Index.cshtml", paged);
    }



   [HttpGet]
    public IActionResult POSTransactionActionHistory(int id)
    {
        var payments = _context.Payment
            .Where(p => p.Payment_ID == id) // <- filter by Payment_ID
            .Include(p => p.Orders)
                .ThenInclude(o => o.customer)
                    .ThenInclude(c => c.barangay)
            .Include(p => p.Orders)
                .ThenInclude(o => o.userAccount)
            .ToList();

        if (!payments.Any())
        {
            return NotFound();
        }

        return PartialView("~/Views/Monitoring/POSTransaction/ActionHistory.cshtml", payments);
    }






    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// Credit / Balanced Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult Credit(
        int id, int page = 1, int pageSize = 5, int window = 1, string search = "",
        int? selectedBarangayID = null, string dateFrom = "", string dateTo = ""
    )
    {
        // Step 1: Load payments with related entities into memory
        var paymentsFromDb = _context.Payment
            .Include(p => p.Orders)
                .ThenInclude(o => o.customer)
                    .ThenInclude(c => c.barangay)
            .ToList(); // EF-safe, now we can use null propagation and statement bodies

        // Step 2: Project into OrderDisplayItem
        var payments = paymentsFromDb
            .Select(payment =>
            {
                var firstOrder = payment.Orders.FirstOrDefault();
                var cust = firstOrder?.customer;
                var barangay = cust?.barangay;

                return new OrderDisplayItem
                {
                    PaymentID = payment.Payment_ID,
                    CustomerName = cust == null
                        ? ""
                        : $"{cust.First_Name} {cust.Last_Name}".Trim(),
                    CustomerAddress = string.Join(", ",
                        new[] { cust?.Street, barangay?.Name }
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                    ),
                    Barangay_ID = barangay?.Barangay_ID,  // nullable int
                    Service = payment.Service,
                    Quantity = payment.Quantity ?? 0,      // handle int? safely
                    Total = payment.Total,
                    Balanced = payment.Balanced ?? 0,
                    Note = payment.Note,
                    Date = payment.Date
                };
            })
            .ToList();

        // Step 3: Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.ToLower().Trim();
            payments = payments
                .Where(p => 
                    p.CustomerName.ToLower().Contains(keyword) ||
                    p.CustomerAddress.ToLower().Contains(keyword))
                .ToList();
        }

        // Step 4: Filter by selected Barangay
        if (selectedBarangayID.HasValue)
        {
            payments = payments
                .Where(p => p.Barangay_ID == selectedBarangayID)
                .ToList();
        }

        // Step 5: Filter by date range
        if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var df))
        {
            var from = DateOnly.FromDateTime(df);
            payments = payments.Where(p => p.Date >= from).ToList();
        }

        if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var dt))
        {
            var to = DateOnly.FromDateTime(dt);
            payments = payments.Where(p => p.Date <= to).ToList();
        }

        // Step 6: Pagination calculations
        int total = payments.Count;
        int totalPages = (int)Math.Ceiling((double)total / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var paged = payments
            .OrderBy(p => p.PaymentID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Step 7: Pass pagination and filter info to the view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        // Step 8: Prepare Barangay list for dropdown
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

        return View("~/Views/Monitoring/Credit/Index.cshtml", paged);
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

    public IActionResult TopCustomer(
        int id,
        int page = 1,
        int pageSize = 5,
        int window = 1,
        string search = "",
        int? selectedBarangayID = null,
        string dateFrom = "",
        string dateTo = "")
    {
        // Step 1: Load payments with related data
        var payments = _context.Payment
            .Include(p => p.Orders)
                .ThenInclude(o => o.customer)
                    .ThenInclude(c => c.barangay)
            .Include(p => p.Orders)
                .ThenInclude(o => o.userAccount)
            .ToList();

        // Step 2: Flatten payments into orders with customers
        var paymentOrders = payments
            .SelectMany(p => p.Orders.Select(o => new { Payment = p, Customer = o.customer }))
            .Where(x => x.Customer != null);

        // Step 3: Apply date filters before aggregation
        if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var df))
        {
            var from = DateOnly.FromDateTime(df);
            paymentOrders = paymentOrders.Where(x => x.Payment.Date >= from);
        }

        if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var dt))
        {
            var to = DateOnly.FromDateTime(dt);
            paymentOrders = paymentOrders.Where(x => x.Payment.Date <= to);
        }

        // Step 4: Group by customer and aggregate
        var customerPayments = paymentOrders
            .GroupBy(x => x.Customer.Customer_ID)
            .Select(g =>
            {
                var cust = g.First().Customer;
                var barangay = cust.barangay;

                // Get latest payment for PaymentID and LastPurchased
                var lastPayment = g.OrderByDescending(x => x.Payment.Date).First().Payment;

                return new OrderDisplayItem
                {
                    PaymentID = lastPayment.Payment_ID,
                    CustomerName = $"{cust.First_Name} {cust.Last_Name}".Trim(),
                    CustomerAddress = string.Join(", ",
                        new[] { cust.Street, barangay?.Name }
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                    ),
                    Barangay_ID = barangay?.Barangay_ID,
                    Quantity = g.Sum(x => x.Payment.Quantity ?? 0),
                    Total = g.Sum(x => x.Payment.Total),
                    LastPurchased = lastPayment.Date
                };
            })
            .ToList();

        // Step 5: Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.ToLower().Trim();
            customerPayments = customerPayments
                .Where(p => p.CustomerName.ToLower().Contains(keyword) ||
                            p.CustomerAddress.ToLower().Contains(keyword))
                .ToList();
        }

        // Step 6: Filter by selected Barangay
        if (selectedBarangayID.HasValue)
        {
            customerPayments = customerPayments
                .Where(p => p.Barangay_ID == selectedBarangayID)
                .ToList();
        }

        // Step 7: Pagination
        int total = customerPayments.Count;
        int totalPages = (int)Math.Ceiling((double)total / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var paged = customerPayments
            .OrderBy(p => p.CustomerName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Step 8: Pass pagination and filter info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

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

        return View("~/Views/Monitoring/TopCustomer/Index.cshtml", paged);
    }





    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// Need Deliver Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult NeedDeliver(
        int id,
        int page = 1,
        int pageSize = 5,
        int window = 1,
        string search = "",
        int? selectedBarangayID = null,
        string dateFrom = "",
        string dateTo = "")
    {
        var payments = _context.Payment
            .Include(p => p.Orders)
                .ThenInclude(o => o.customer)
                    .ThenInclude(c => c.barangay)
            .Include(p => p.Orders)
            .ToList();

        var paymentOrders = payments   
            .SelectMany(p => p.Orders.Select(o => new { Payment = p, Customer = o.customer }))
            .Where(x => x.Customer !=null);

        if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var df))
        {
            var from = DateOnly.FromDateTime(df);
            paymentOrders = paymentOrders.Where(x => x.Payment.Date >= from);
        }


        if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var dt))
        {
            var to = DateOnly.FromDateTime(dt);
            paymentOrders = paymentOrders.Where(x => x.Payment.Date <=to);
        } 

        return View("~/Views/Monitoring/NeedDeliver/Index.cshtml");
    }


    

 
}
