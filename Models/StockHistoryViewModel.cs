using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    public class StockHistoryViewModel
    {
        public int ID { get; set; }
        public int  Quantity { get; set; }
        public int Price { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get;  set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}