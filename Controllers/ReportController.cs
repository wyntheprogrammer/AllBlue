using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AllBlue.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AllBlue.Controllers;

public class ReportController : Controller
{
    private readonly ILogger<ReportController> _logger;
    private readonly AppDbContext _context;

    public ReportController(ILogger<ReportController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(int? month, int? year, int monthlyPage = 1, int monthlyPageSize = 10, int monthlyWindow = 1, int yearlyPage = 1, int yearlyPageSize = 10, int yearlyWindow = 1)
    {
        // Default to current month and year if not provided
        int selectedMonth = month ?? DateTime.Now.Month;
        int selectedYear = year ?? DateTime.Now.Year;

        // Generate month list
        var months = Enumerable.Range(1, 12).Select(m => new 
        { 
            Value = m, 
            Text = new DateTime(2000, m, 1).ToString("MMMM") 
        }).ToList();

        // Generate year list (current year - 5 to current year + 1)
        var years = Enumerable.Range(DateTime.Now.Year - 5, 7).ToList();

        ViewBag.Months = months;
        ViewBag.Years = years;
        ViewBag.SelectedMonth = selectedMonth;
        ViewBag.SelectedYear = selectedYear;

        // Get monthly report data with pagination
        var monthlyReport = GetMonthlyReport(selectedMonth, selectedYear, monthlyPage, monthlyPageSize, monthlyWindow);
        
        // Get yearly report data with pagination
        var yearlyReport = GetYearlyReport(selectedYear, yearlyPage, yearlyPageSize, yearlyWindow);

        var viewModel = new ReportViewModel
        {
            MonthlyReport = monthlyReport,
            YearlyReport = yearlyReport
        };

        return View(viewModel);
    }

    private MonthlyReportData GetMonthlyReport(int month, int year, int page, int pageSize, int window)
    {
        var startDate = new DateOnly(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var payments = _context.Payment
            .Include(p => p.Orders)
                .ThenInclude(o => o.userAccount)
            .Where(p => p.Date >= startDate && p.Date <= endDate)
            .ToList();

        // Calculate totals
        var deliverCount = payments.Where(p => p.Service == "Deliver").Sum(p => p.Quantity ?? 0);
        var pickupCount = payments.Where(p => p.Service == "Pickup").Sum(p => p.Quantity ?? 0);
        var totalSales = payments.Sum(p => p.Total);
        var totalUnpaid = payments.Sum(p => p.Balanced ?? 0);
        var totalVoid = 0; // Add void logic if needed
        var totalExpenses = GetExpensesForPeriod(startDate, endDate);

        // Group by day
        var dailyDataQuery = payments
            .GroupBy(p => p.Date)
            .Select(g => new DailyReportItem
            {
                Date = g.Key,
                DeliverCount = g.Where(p => p.Service == "Deliver").Sum(p => p.Quantity ?? 0),
                PickupCount = g.Where(p => p.Service == "Pickup").Sum(p => p.Quantity ?? 0),
                FreeCount = g.Sum(p => p.Free ?? 0),
                Quantity = g.Sum(p => p.Quantity ?? 0),
                Total = g.Sum(p => p.Total),
                Unpaid = g.Sum(p => p.Balanced ?? 0),
                Expenses = 0, // Add expenses logic
                Net = g.Sum(p => p.Total) - 0, // Total - Expenses
                InCharge = string.Join(", ", g.SelectMany(p => p.Orders)
                    .Select(o => o.userAccount?.Username)
                    .Distinct()
                    .Where(u => u != null))
            })
            .OrderBy(d => d.Date)
            .ToList();

        // Pagination
        int totalDays = dailyDataQuery.Count();
        int totalPages = (int)Math.Ceiling((double)totalDays / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var pagedDailyData = dailyDataQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.MonthlyCurrentPage = page;
        ViewBag.MonthlyTotalPages = totalPages;
        ViewBag.MonthlyStartPage = startPage;
        ViewBag.MonthlyEndPage = endPage;
        ViewBag.MonthlyWindow = window;

        return new MonthlyReportData
        {
            DeliverGallons = deliverCount,
            PickupGallons = pickupCount,
            TotalSales = totalSales,
            TotalUnpaid = totalUnpaid,
            TotalVoid = totalVoid,
            TotalExpenses = totalExpenses,
            DailyData = pagedDailyData
        };
    }

    private YearlyReportData GetYearlyReport(int year, int page, int pageSize, int window)
    {
        var startDate = new DateOnly(year, 1, 1);
        var endDate = new DateOnly(year, 12, 31);

        var payments = _context.Payment
            .Include(p => p.Orders)
                .ThenInclude(o => o.userAccount)
            .Where(p => p.Date >= startDate && p.Date <= endDate)
            .ToList();

        // Calculate yearly totals
        var deliverCount = payments.Where(p => p.Service == "Deliver").Sum(p => p.Quantity ?? 0);
        var pickupCount = payments.Where(p => p.Service == "Pickup").Sum(p => p.Quantity ?? 0);
        var totalSales = payments.Sum(p => p.Total);
        var totalUnpaid = payments.Sum(p => p.Balanced ?? 0);
        var totalVoid = 0;
        var totalExpenses = GetExpensesForPeriod(startDate, endDate);

        // Group by month
        var monthlyDataQuery = payments
            .GroupBy(p => p.Date.Month)
            .Select(g => new MonthlyReportItem
            {
                Month = new DateTime(year, g.Key, 1).ToString("MMMM"),
                MonthNumber = g.Key,
                DeliverCount = g.Where(p => p.Service == "Deliver").Sum(p => p.Quantity ?? 0),
                PickupCount = g.Where(p => p.Service == "Pickup").Sum(p => p.Quantity ?? 0),
                FreeCount = g.Sum(p => p.Free ?? 0),
                Quantity = g.Sum(p => p.Quantity ?? 0),
                Total = g.Sum(p => p.Total),
                Unpaid = g.Sum(p => p.Balanced ?? 0),
                Expenses = 0,
                Net = g.Sum(p => p.Total) - 0,
                InCharge = string.Join(", ", g.SelectMany(p => p.Orders)
                    .Select(o => o.userAccount?.Username)
                    .Distinct()
                    .Where(u => u != null))
            })
            .OrderBy(m => m.MonthNumber)
            .ToList();

        // Pagination
        int totalMonths = monthlyDataQuery.Count();
        int totalPages = (int)Math.Ceiling((double)totalMonths / pageSize);

        int windowSize = 5;
        int startPage = ((window - 1) * windowSize) + 1;
        int endPage = Math.Min(startPage + windowSize - 1, totalPages);

        var pagedMonthlyData = monthlyDataQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.YearlyCurrentPage = page;
        ViewBag.YearlyTotalPages = totalPages;
        ViewBag.YearlyStartPage = startPage;
        ViewBag.YearlyEndPage = endPage;
        ViewBag.YearlyWindow = window;

        return new YearlyReportData
        {
            DeliverGallons = deliverCount,
            PickupGallons = pickupCount,
            TotalSales = totalSales,
            TotalUnpaid = totalUnpaid,
            TotalVoid = totalVoid,
            TotalExpenses = totalExpenses,
            MonthlyData = pagedMonthlyData
        };
    }

    private decimal GetExpensesForPeriod(DateOnly startDate, DateOnly endDate)
    {
        // TODO: Implement expenses calculation when expenses table is available
        // For now, return 0
        return 0;
    }
}

