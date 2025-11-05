using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AllBlue.Controllers;

public class UserAccountController : Controller
{
    private readonly ILogger<UserAccountController> _logger;
    private readonly AppDbContext _context;

    public UserAccountController(ILogger<UserAccountController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(string search, int page = 1, int pageSize = 5, int window = 1)
    {
        var user = _context.UserAccount.AsQueryable();

        if (!string.IsNullOrEmpty(search?.Trim()))
        {
            string keyword = search.Trim().ToLower();
            user = user.Where(u => u.Firstname.ToLower().Contains(keyword));
        }

        int totalUser = user.Count();
        int totalPages = (int)Math.Ceiling((double)totalUser / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var pagedUser = user 
            .OrderBy(u => u.User_Account_ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(u => u.accountType)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;

        return View(pagedUser);
    }


    [HttpGet]
    public IActionResult AddUser()
    {
        var accountTypeList = _context.AccountType.Select(a => new SelectListItem
        {
            Value = a.Account_Type_ID.ToString(),
            Text = a.Title 
        }).ToList();

        ViewBag.AccountTypeList = new SelectList(accountTypeList, "Value", "Text");

        return View("AddUser");
    }   

    [HttpPost]
    public async Task<IActionResult> AddUser(UserAccount userAccount, IFormFile Image)
    {
        if (ModelState.IsValid)
        {
            try
            {
                 if (Image != null && Image.Length > 0) 
                 {
                     var uniqueFileName = userAccount.Lastname + DateTime.Now.ToString("_yyyyMMddHHmmssfff") + Path.GetExtension(Image.FileName);
                     var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/User", uniqueFileName);

                     using (var stream = new FileStream(filePath, FileMode.Create))
                     {
                         await Image.CopyToAsync(stream);
                     }

                     userAccount.Image = uniqueFileName;
                }

                userAccount.Password = "123";
                userAccount.Status = "Active";
                
                _context.UserAccount.Add(userAccount);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User Account added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
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



    


    [HttpGet]
    public IActionResult EditUser(int id)
    {
        var user = _context.UserAccount.FirstOrDefault(u => u.User_Account_ID == id);
        if (user ==  null)
        {
            return NotFound();
        }

        var accountTypeList = _context.AccountType.Select(a => new SelectListItem
        {
            Value = a.Account_Type_ID.ToString(),
            Text = a.Title 
        }).ToList();

        ViewBag.AccountTypeList = new SelectList(accountTypeList, "Value", "Text");


        return PartialView("EditUser", user);
    }


    [HttpPost]
    public async Task<IActionResult> EditUser(UserAccount userAccount, IFormFile Image)
    {
        var existingUser = _context.UserAccount.FirstOrDefault(u => u.User_Account_ID == userAccount.User_Account_ID);
        if(existingUser == null) 
        {
            TempData["ErrorMessage"] = "Failed to update user";
            return NotFound();
        }

        try {
            if (Image != null && Image.Length > 0) 
            {
                var uniqueFileName = userAccount.Lastname + DateTime.Now.ToString("_yyyyMMddHHmmssfff") + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/User", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                existingUser.Image = uniqueFileName;
            }

            existingUser.Password = "123";
            existingUser.Status = "Active";
            

            existingUser.Username = userAccount.Username;
            existingUser.Email = userAccount.Email;
            existingUser.Contact = userAccount.Contact;
            existingUser.Gender = userAccount.Gender;
            existingUser.Address = userAccount.Address;
            existingUser.User_Account_ID = userAccount.User_Account_ID;
            existingUser.Status = userAccount.Status;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "User Account updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        // If ModelState is invalid, collect all errors
        var errors = ModelState
            .Where(kvp => kvp.Value.Errors.Count > 0)
            .Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))}")
            .ToList();

        TempData["ErrorMessage"] = string.Join(" | ", errors);

        return RedirectToAction("Index");

    }




    [HttpGet]
    public IActionResult DeactivateUser(int id)
    {
        var userAccount = _context.UserAccount.FirstOrDefault(u => u.User_Account_ID == id);
        if (userAccount == null)
        {
            return NotFound();
        }

        return PartialView("DeactivateUser", userAccount);
    }


    [HttpPost]
    public IActionResult DeactivateUser(UserAccount userAccount)
    {
        var existingUser = _context.UserAccount.FirstOrDefault(u => u.User_Account_ID == userAccount.User_Account_ID);
        if(existingUser == null)
        {
            TempData["ErrorMessage"] = "Failed to deactivate user.";
            return NotFound();
        }

        try
        {
            existingUser.Status = userAccount.Status;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "User Account deactivated successfully.";       
        } catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Index");
    }




    [HttpGet]
    public IActionResult ChangePassword(int id)
    {
        var userAccount = _context.UserAccount.FirstOrDefault(u => u.User_Account_ID == id);
        if (userAccount == null)
        {
            return NotFound();
        }

        
        return PartialView("ChangePassword", userAccount);
    }

    [HttpPost]
    public IActionResult ChangePassword(UserAccount userAccount)
    {
        var existingUser = _context.UserAccount.FirstOrDefault(u => u.User_Account_ID == userAccount.User_Account_ID);
        if (existingUser == null)
        {
            TempData["ErrorMessage"] = "Failed to change password.";
            return NotFound();
        }

        try 
        {
            existingUser.Password = userAccount.Password;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Password updated successfully!";
        } catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Index");

    }




    [HttpGet]
    public IActionResult DeleteUser(int id)
    {
        var userAccount = _context.UserAccount.FirstOrDefault(u => u.User_Account_ID == id);
        if (userAccount == null)
        {
            return NotFound();
        }

        return PartialView("Index", userAccount);
    }

    [HttpPost]
    public IActionResult DeleteUser(UserAccount userAccount)
    {
        var existingUser = _context.UserAccount.FirstOrDefault(u => u.User_Account_ID == userAccount.User_Account_ID);
        if (existingUser == null)
        {
            TempData["ErrorMessage"] = "Failed to delete user.";
            return NotFound();
        }

        try 
        {
            _context.UserAccount.Remove(existingUser);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "User Account deleted successfully!";
        } catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

}
 