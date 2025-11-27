using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    public class  OrderProductItem
    {
        public string ItemName { get; set; }
        public int ClientGal { get; set; }
        public int WRSGal { get; set; }
        public int QTY { get; set; }
        public int FreeGal { get; set; }
        public string Price { get; set; }

    }

    public class OrderDisplayItem
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string UserName { get; set; }
        public List<OrderProductItem> Products { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
        public DateOnly Date {  get; set; }
    }

    // ViewModel for the Index page
    public class IndexViewModel
    {
        public List<Item> Items { get; set; }
        public List<OrderDisplayItem> Orders { get; set; }
    }
}