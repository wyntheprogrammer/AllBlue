using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class AccountTypeController : Controller
{
    private readonly ILogger<AccountTypeController> _logger;
    private readonly AppDbContext _context;

    public AccountTypeController(ILogger<AccountTypeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(string search, int page = 1, int pageSize = 5, int window = 1)
    {
        var account = _context.AccountType.AsQueryable();

        if (!string.IsNullOrEmpty(search?.Trim()))
        {
            string keyword = search.Trim().ToLower();
            account = account.Where(a => a.Title.ToLower().Contains(keyword));
        }

        int totalAccount = account.Count();
        int totalPages = (int)Math.Ceiling((double)totalAccount / pageSize);


        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var pagedAccount = account
            .OrderBy(a => a.Account_Type_ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;

        return View(pagedAccount);
    }

    [HttpGet]
    public IActionResult AddAccount()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddAccount(AccountType accountType, List<string> Menu)
    {
        if (ModelState.IsValid)
        {
            try
            {
                accountType.Menu = string.Join(",", Menu);
                _context.AccountType.Add(accountType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Account added successfully!.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }


    [HttpGet]
    public IActionResult EditAccount(int id)
    {   
        var account = _context.AccountType.FirstOrDefault(a => a.Account_Type_ID == id);
        if (account == null)
        {
            return NotFound();
        }

        return PartialView("EditAccount", account);
    }
    
    [HttpPost]
    public IActionResult EditAccount(AccountType accountType, List<string> Menu)
    {
        var existingAccount = _context.AccountType.FirstOrDefault(a => a.Account_Type_ID == accountType.Account_Type_ID);
        if (existingAccount == null)
        {
            TempData["ErrorMessage"] = "Failed to update account.";
            return NotFound();
        }

        try {
            existingAccount.Title = accountType.Title;
            existingAccount.Description = accountType.Description;
            existingAccount.Menu = string.Join(",", Menu);

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Account updated successfully!.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Index");
    }


    [HttpGet]
    public IActionResult DeleteAccount(int id)
    {
        var account = _context.AccountType.FirstOrDefault(a => a.Account_Type_ID == id);
        if(account == null)
        {
            return NotFound();
        }

        return View("DeleteAccount", account);
    }


    [HttpPost]
    public IActionResult DeleteAccount (AccountType accountType)
    {
        var existingAccount = _context.AccountType.FirstOrDefault(a => a.Account_Type_ID == accountType.Account_Type_ID);
        if (existingAccount == null)
        {
            TempData["ErrorMessage"] = "Account not found.";

            return NotFound();
        }

        try
        {
            _context.AccountType.Remove(existingAccount);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Account Type deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

}
