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

    public IActionResult Expenses(string search, int page = 1, int pageSize = 5, int window = 1, int month = 1, int year = 2019)
    {
        var query = _context.ExpenseCategory
            .Select(c => new ExpenseMonthlyViewModel
            {
                ExpenseCategoryID = c.ExpenseCategoryID,
                CategoryName = c.Name,

                Jan = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 1).Sum(e => e.TotalValue),
                Feb = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 2).Sum(e => e.TotalValue),
                Mar = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 3).Sum(e => e.TotalValue),
                Apr = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 4).Sum(e => e.TotalValue),
                May = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 5).Sum(e => e.TotalValue),
                Jun = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 6).Sum(e => e.TotalValue),
                Jul = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 7).Sum(e => e.TotalValue),
                Aug = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 8).Sum(e => e.TotalValue),
                Sep = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 9).Sum(e => e.TotalValue),
                Oct = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 10).Sum(e => e.TotalValue),
                Nov = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 11).Sum(e => e.TotalValue),
                Dec = c.Expenses.Where(e => e.Date.Year == year && e.Date.Month == 12).Sum(e => e.TotalValue),            
            
                YearlyTotal = c.Expenses
                    .Where(e => e.Date.Year == year)
                    .Sum(e => e.TotalValue)
            });

        if (!string.IsNullOrEmpty(search?.Trim()))
        {
            string keyword = search.Trim().ToLower();
            query = query.Where(e => e.CategoryName.ToLower().Contains(keyword));
        }

        var allExpenses = query.ToList();

        int totalExpense = allExpenses.Count();
        int totalPages = (int)Math.Ceiling((double)totalExpense / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var pagedExpense = allExpenses 
            .OrderBy(e => e.ExpenseCategoryID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();


        int monthlyTotal = _context.Expense
            .Where(e => e.Date.Year == year && e.Date.Month == month)
            .Sum(e => (int?)e.TotalValue) ?? 0;
        
        int yearlyTotal = _context.Expense
            .Where(e => e.Date.Year == year)
            .Sum(e => (int?)e.TotalValue) ?? 0;


        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;


        ViewBag.MonthlyTotal = monthlyTotal;
        ViewBag.YearlyTotal = yearlyTotal;

        ViewBag.SelectedMonth = month;
        ViewBag.SelectedYear = year;

        return View("~/Views/Stock/Expenses/Index.cshtml", pagedExpense);
    }

    [HttpGet]
    public IActionResult AddExpenses()
    {
        return View("~/Views/Stock/Expenses/AddExpenses.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> AddExpenses(Expense expense)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Expense.Add(expense);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Expense added successfully.";
                return RedirectToAction("Expenses");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }
        return RedirectToAction("Expenses");
    }


    public IActionResult ExpensesHistory(int id)
    {
        var category = _context.ExpenseCategory
            .FirstOrDefault(x => x.ExpenseCategoryID == id );

        if (category == null)
        {
            return Content("<div class='p-3 text-danger'>Category not found. </div>");
        }

        var items = _context.Expense
            .Where(x => x.ExpenseCategoryID == id)
            .OrderByDescending(x => x.Date)
            .Select(x => new ExpensesHistoryItemViewModel
            {
                ExpenseID = x.ExpenseID,
                Qty = 1,
                Price = x.TotalValue,
                Comment = x.Comment,
                Status = "In",
                Transaction = "New Stock",
                Date = x.Date.ToString("yyyy-MM-dd")
            }).ToList();

        var vm = new ExpensesHistoryViewModel{
            CategoryName = category.Name,
            Items = items
        };


        return PartialView("~/Views/Stock/Expenses/ExpensesHistory.cshtml", vm);
    }

}
