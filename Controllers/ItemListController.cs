using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;

namespace AllBlue.Controllers;

public class ItemListController : Controller
{
    private readonly ILogger<ItemListController> _logger;
    private readonly AppDbContext _context;

    public ItemListController(ILogger<ItemListController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(string search, int page = 1, int pageSize = 5, int window = 1)
    {
        var item = _context.Item.AsQueryable();

        if (!string.IsNullOrEmpty(search?.Trim()))
        {
            string keyword = search.Trim().ToLower();
            item = item.Where(i => i.Title.ToLower().Contains(keyword));
        }

        int totalItem = item.Count();
        int totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var pagedItem = item
            .OrderBy(i => i.Item_ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.StartPage = startPage;
        ViewBag.EndPage = endPage;
        ViewBag.Window = window;
        ViewBag.Search = search;

        return View(pagedItem);
    }


    [HttpGet]
    public IActionResult AddItem()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(Item item, IFormFile Image){
        if (ModelState.IsValid)
        {
            try
            {
                if (Image != null && Image.Length > 0)
                {
                    var uniqueFileName = item.Title + DateTime.Now.ToString("_yyyyMMddHHmmssfff") + Path.GetExtension(Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Item", uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    item.Image = uniqueFileName;
                }

                item.Date = DateTime.Today;

                _context.Item.Add(item);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Item added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }
        
        return RedirectToAction("Index"); 

    }

    [HttpGet]
    public IActionResult EditItem(int id)
    {
        var item = _context.Item.FirstOrDefault(i => i.Item_ID == id);
        if (item == null)
        {
            return NotFound();
        }
        return View("EditItem", item);
    }

    [HttpPost]
    public async Task<IActionResult> EditItem(Item item, IFormFile Image)
    {
        var existingItem = _context.Item.FirstOrDefault(i => i.Item_ID == item.Item_ID);
        if (existingItem == null)
        {
            TempData["ErrorMessage"] = "Failed to update item.";
            return NotFound();
        }

        try
        {
            if (Image != null && Image.Length > 0)
            {
                var uniqueFileName = item.Title + DateTime.Now.ToString("_yyyyMMddHHmmssfff") + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Item", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                existingItem.Image = uniqueFileName;
            }

            existingItem.Date = DateTime.Today;

            existingItem.Title = item.Title;
            existingItem.Item_Type = item.Item_Type;
            existingItem.POS_Item = item.POS_Item;
            existingItem.Reorder = item.Reorder;
            existingItem.Deliver = item.Deliver;
            existingItem.Pickup = item.Pickup;
            existingItem.Buy = item.Buy;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Item updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Index");
    }


    [HttpGet]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Item.FirstOrDefault(i => i.Item_ID == id);
        if (item == null)
        {
            return NotFound();
        }

        return View("DeleteItem", item);
    }

    [HttpPost]
    public IActionResult DeleteItem (Item item)
    {
        var existingItem = _context.Item.FirstOrDefault(i => i.Item_ID == item.Item_ID);
        if (existingItem == null)
        {
            TempData["ErrorMessage"] = "Item not found.";

            return NotFound();
        }

        try
        {
            _context.Item.Remove(existingItem);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Item deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Index");
    }
}
