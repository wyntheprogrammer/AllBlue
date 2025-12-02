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
        public int Price { get; set; }

    }

    public class OrderDisplayItem
    {
        public int? PaymentID { get; set;}
        public int? OrderID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string UserName { get; set; }

        public List<OrderProductItem> Products { get; set; }
        
        public int Quantity { get; set;}
        public int? Free { get; set;}
        public string Discount { get; set;}
        public int Cash { get; set; }
        public int Changed { get; set; }
        public int Balanced  { get; set;}
        public int Total { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateOnly Date {  get; set; }
        public DateOnly LastPurchased { get; set; }


        public int? Barangay_ID { get; set;}
    }

    // ViewModel for the Index page
    public class IndexViewModel
    {
        public List<Item> Items { get; set; }
        public List<OrderDisplayItem> Orders { get; set; }
    }
}