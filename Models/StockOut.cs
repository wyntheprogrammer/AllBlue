using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("StockOut")]
    public class StockOut
    {
        [Key]
        public int StockOut_ID { get; set; }
        public int Stock_ID { get; set; }
        public int  Quantity { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get;  set; }

        [ForeignKey("Stock_ID")]
        [ValidateNever]
        public required Stock stock { get; set; }
    }
}