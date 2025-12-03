using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllBlue.Controllers;

public class StockController : Controller
{
    private readonly ILogger<StockController> _logger;
    private readonly AppDbContext _context;

    public StockController(ILogger<StockController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }


    ////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////// Stock Module /////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////

    public IActionResult Stock(string search, int page = 1, int pageSize = 5, int window = 1)
    {
        var stock = _context.Stock.AsQueryable();

        if(!string.IsNullOrEmpty(search?.Trim()))
        {
            string keyword = search.Trim().ToLower();
            stock = stock.Where(s => s.item.Title.ToLower().Contains(keyword));
        }

        int totalStock = stock.Count();
        int totalPages = (int)Math.Ceiling((double)totalStock / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);


        var pagedStock = stock
            .OrderBy(s => s.Stock_ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(s => s.item)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;

        return View("~/Views/Stock/Stock/Index.cshtml", pagedStock);
    }

    [HttpGet]
    public IActionResult StockIn(int id)
    {
        var stock = _context.Stock
            .Include(s => s.item)
            .FirstOrDefault(s => s.Stock_ID == id);
        if (stock == null)
        {
            return NotFound();
        }
        
        return View("~/Views/Stock/Stock/StockIn.cshtml", stock);
    }

    [HttpPost]
    public async Task<IActionResult> StockIn(StockIn stockIn)
    {
        if (ModelState.IsValid)
        {
            try 
            {
                _context.StockIn.Add(stockIn);
                await _context.SaveChangesAsync();

                var stock = await _context.Stock
                    .FirstOrDefaultAsync(s => s.Stock_ID == stockIn.Stock_ID);

                if (stock != null)
                {
                    stock.In += stockIn.Quantity;
                    stock.OnHand += stockIn.Quantity;

                    _context.Stock.Update(stock);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Stock updated successfully.";
                return RedirectToAction("Stock");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }

        return RedirectToAction("Stock");
    }


    [HttpPost]
    public async Task<IActionResult> StockOut(StockOut stockOut)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.StockOut.Add(stockOut);
                await _context.SaveChangesAsync();

                var stock = await _context.Stock
                    .FirstOrDefaultAsync(s => s.Stock_ID == stockOut.Stock_ID);
                
                if (stock != null)
                {
                    if (stock.OnHand < stockOut.Quantity)
                    {
                        TempData["ErrorMessage"] = "Not enough stock available.";
                        return RedirectToAction("Stock");
                    }

                    stock.Out += stockOut.Quantity;
                    stock.OnHand -= stockOut.Quantity;

                    _context.Stock.Update(stock);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Stock deducted successfully.";
                return RedirectToAction("Stock");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }

        return RedirectToAction("Stock");
    }

    [HttpGet]
    public IActionResult StockOut(int id)
    {
        var stock = _context.Stock
            .Include(s => s.item)
            .FirstOrDefault(s => s.Stock_ID == id);
        if (stock == null)
        {
            return NotFound();
        }

        return View("~/Views/Stock/Stock/StockOut.cshtml", stock);
    }
    

    // [HttpGet]    
    // public IActionResult StockHistory(int id)
    // {
    //     var stock = _context.Stock.FirstOrDefault(s => s.Stock_ID == id);
    //     if (stock == null)
    //     {
    //         return NotFound();
    //     }

    //     return View("~/Views/Stock/Stock/StockHistory.cshtml", stock);
    // }

    // [HttpPost]

    public async Task<IActionResult> StockHistory(int id)
    {
        var stock = await _context.Stock
            .Include(s => s.item)
            .FirstOrDefaultAsync(s => s.Stock_ID == id);

        if (stock == null) return NotFound();

        var stockIn = await _context.StockIn
            .Where(x => x.Stock_ID == id)
            .Select(x => new StockHistoryViewModel
            {
                ID = x.StockIn_ID,
                Quantity = x.Quantity,
                Price = x.Price,
                Comment = x.Comment,
                Date = x.Date,
                Type = "IN",
                Status = "In"
            }).ToListAsync();

        var stockOut = await _context.StockOut
            .Where(x => x.Stock_ID == id)
            .Select(x => new StockHistoryViewModel
            {
                ID = x.StockOut_ID,
                Quantity = x.Quantity,
                Price = 0,
                Comment = x.Comment,
                Date = x.Date,
                Type = "OUT",
                Status = x.Status
            }).ToListAsync();

        var history = stockIn.Concat(stockOut)
                            .OrderByDescending(x => x.Date)
                            .ToList();

        var viewModel = new StockHistoryPageViewModel
        {
            Stock = stock,
            History = history
        };

        return PartialView("~/Views/Stock/Stock/StockHistory.cshtml", viewModel);
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
