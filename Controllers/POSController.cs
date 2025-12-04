using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;
using SQLitePCL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using DevExpress.Data;

namespace AllBlue.Controllers;

public class POSController : Controller
{
    private readonly ILogger<POSController> _logger;
    private readonly AppDbContext _context;

    public POSController(ILogger<POSController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(int? id, int page = 1, int pageSize = 5, int window = 1)
    {
        if (id.HasValue)
        {
            var customer = _context.Customer
                .Include(c => c.barangay)
                .Include(c => c.city)
                .FirstOrDefault(c => c.Customer_ID == id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.CustomerID = customer.Customer_ID;
            ViewBag.CustomerName = $"{customer.First_Name} {customer.Last_Name}";
            ViewBag.CustomerAddress = $"{customer.barangay.Name}, {customer.city.Name}";
        }
        else
        {
            ViewBag.CustomerName = "Guest";
        }

        var items = _context.Item.ToList();

        var userList = _context.UserAccount
            .Where(u => u.Account_Type_ID == 4)
            .Select(u => new SelectListItem
            {
                Value = u.User_Account_ID.ToString(),
                Text = u.Username
            }).ToList();

        ViewBag.UserList = new SelectList(userList, "Value", "Text");


        var orders = _context.Order
            .Include(o => o.customer)
            .Include(o => o.payment)
            .Include(o => o.item)
            .Include(o => o.userAccount)
            .ToList()
            .GroupBy(o => o.Payment_ID)
            .Select(g => new OrderDisplayItem
            {
                OrderID = g.Key,
                CustomerName = g.First().customer.First_Name,
                CustomerAddress = g.First().customer.Street,
                UserName = g.First().userAccount.Username,
                Date = g.First().payment.Date,
                Service = g.First().payment.Service,
                Status = g.First().payment.Status,
                Products = g.Select(o => new OrderProductItem
                {
                    ItemName = o.item.Title,
                    ClientGal = o.Client_Gal ?? 0,  
                    WRSGal = o.WRS_Gal ?? 0,
                    QTY = o.Quantity ?? 0,
                    FreeGal = o.Free_Gal ?? 0,
                    Price = o.Total ?? 0
                }).ToList()

            }).ToList();


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

        var model = new IndexViewModel
        {
            Items = items,
            Orders = pagedOrder
        };

        return View(model);
    }




    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Add Customer Modal ////////////////////////////////// 
    ////////////////////////////////////////////////////////////////////////////////////////
    [HttpGet]
    public IActionResult AddCustomer()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomer(Customer customer)
    {
        if (ModelState.IsValid)
        {
            try 
            {
                _context.Customer.Add(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Customer added successfully!";
                return RedirectToAction("Index");
            }
            catch ( Exception ex )
            {
                TempData["ErrorMessage"] = $"Exception: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // If ModelState is invalid, collect all errors
        var errors = ModelState
            .Where(kvp => kvp.Value.Errors.Count > 0)
            .Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))}")
            .ToList();

        TempData["ErrorMessage"] = string.Join(" | ", errors);

        return RedirectToAction("Index");
    }


    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Search Customer Modal ////////////////////////////////// 
    ////////////////////////////////////////////////////////////////////////////////////////
    public IActionResult SearchCustomer(int page = 1, int pageSize = 5, int window = 1,string keyword ="", int? selectedBarangayID = null, int? selectedCityID =null )
    {

        //Display List of customers
        var cust = _context.Customer
            .Include(c => c.barangay)
            .Include(c => c.city)
            .Include(c => c.item)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalizedKeyword = keyword.Trim().ToLower();

            cust = cust.Where(c =>
                (c.First_Name ?? "").ToLower().Contains(normalizedKeyword) ||
                (c.Last_Name ?? "").ToLower().Contains(normalizedKeyword) ||
                (c.Alias ?? "").ToLower().Contains(normalizedKeyword));
        }


        if (selectedBarangayID.HasValue)
        {
            cust = cust.Where(c => c.Barangay_ID == selectedBarangayID.Value);
        }
        
        if (selectedCityID.HasValue)
        {
            cust = cust.Where(c => c.City_ID == selectedCityID.Value);
        }

        int totalUser = cust.Count();
        int totalPages = (int)Math.Ceiling((double)totalUser / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var customer = cust
             .OrderBy(c => c.Customer_ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;

        var barangay = _context.Barangay.ToList();
        var city = _context.City.ToList();

        var model = new CustomerBarangayCityViewModel
        {
            Customers = customer,
            Barangay = barangay,
            City = city
        };

        ViewBag.barangayList = new SelectList(
            barangay.Select(b => new SelectListItem
            {
                Value = b.Barangay_ID.ToString(),
                Text = b.Name
            }).ToList(), "Value", "Text"
        );


        ViewBag.cityList = new SelectList(
            city.Select(c => new SelectListItem
            {
                Value = c.City_ID.ToString(),
                Text = c.Name
            }).ToList(), "Value", "Text"
        );


        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_SearchCustomerContent", model);
        }

        
        return PartialView("SearchCustomer", model);   
    }




    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Search Customer Modal ////////////////////////////////// 
    ////////////////////////////////////////////////////////////////////////////////////////
    public IActionResult ReturnGallon()
    {
        return View();
    }



    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Search Customer Modal ////////////////////////////////// 
    ////////////////////////////////////////////////////////////////////////////////////////
    public IActionResult ConfirmPayment([FromBody] ConfirmPaymentViewModel model)
    {
       if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

            // Log to console
            foreach (var error in errors)
            {
                Console.WriteLine("ModelState error: " + error);
            }

            return BadRequest(string.Join("; ", errors));
        }


        return View(model);
    }


    [HttpPost]
    public IActionResult SubmitOrder(ConfirmPaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
            .SelectMany(x => x.Value.Errors.Select(err => new { x.Key, err.ErrorMessage }))
            .ToList();

            return Json(errors);
        }

        try
        {
            // 1. Save Payment
            var payment = new Payment
            {
                Quantity = model.TotalQty,
                Free = model.Free,
                Discount = model.Discount,
                Cash = model.Cash,
                Changed = model.Changed,
                Balanced = model.Balanced,
                Total = model.TotalPrice,
                Service = model.SelectedService,
                Status = model.Status,
                Note = model.Note,
                Date = model.Date
            };

            _context.Payment.Add(payment);
            _context.SaveChanges(); // Generate Payment_ID

            // 2. Save Orders
            if (model.Items != null && model.Items.Count > 0)
            {
                foreach (var item in model.Items)
                {
                    try
                    {
                        var order = new Order
                        {
                            Payment_ID = payment.Payment_ID,
                            Customer_ID = model.CustomerID,
                            User_Account_ID = model.SelectedUserID,
                            Item_ID = item.ItemID,
                            Client_Gal = item.ClientGal,
                            WRS_Gal = item.WRSGal,
                            Free_Gal = item.FreeGal,
                            Quantity = item.Qty,
                            Total = item.Total
                        };

                        _context.Order.Add(order);
                        _context.SaveChanges(); // save each order separately

                        TempData["SuccessMessage"] = "Payment added successfully.";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        var msg = $"Failed inserting Order: Payment_ID={payment.Payment_ID}, Customer_ID={model.CustomerID}, User_ID={model.SelectedUserID}, Item_ID={item.ItemID}";
                        return StatusCode(500, $"FK Error: {msg}\nException: {ex.InnerException?.Message}");

                        TempData["ErrorMessage"] = ex.Message;
                    }
                }
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error saving data: {ex.Message} \n {ex.InnerException?.Message}");
        }
    }




}
