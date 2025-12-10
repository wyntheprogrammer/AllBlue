using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{

    public class ReportViewModel
    {
        public MonthlyReportData MonthlyReport { get; set; }
        public YearlyReportData YearlyReport { get; set; }
    }

    public class MonthlyReportData
    {
        public int DeliverGallons { get; set; }
        public int PickupGallons { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalUnpaid { get; set; }
        public decimal TotalVoid { get; set; }
        public decimal TotalExpenses { get; set; }
        public List<DailyReportItem> DailyData { get; set; } = new();
    }

    public class YearlyReportData
    {
        public int DeliverGallons { get; set; }
        public int PickupGallons { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalUnpaid { get; set; }
        public decimal TotalVoid { get; set; }
        public decimal TotalExpenses { get; set; }
        public List<MonthlyReportItem> MonthlyData { get; set; } = new();
    }

    public class DailyReportItem
    {
        public DateOnly Date { get; set; }
        public int DeliverCount { get; set; }
        public int PickupCount { get; set; }
        public int FreeCount { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal Unpaid { get; set; }
        public decimal Expenses { get; set; }
        public decimal Net { get; set; }
        public string InCharge { get; set; }
    }

    public class MonthlyReportItem
    {
        public string Month { get; set; }
        public int MonthNumber { get; set; }
        public int DeliverCount { get; set; }
        public int PickupCount { get; set; }
        public int FreeCount { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal Unpaid { get; set; }
        public decimal Expenses { get; set; }
        public decimal Net { get; set; }
        public string InCharge { get; set; }
    }
}