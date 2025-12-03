using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("Stock")]
    public class Stock
    {
        [Key]
        public int Stock_ID { get; set; }
        public int  Item_ID { get; set; }
        public int In { get; set; }
        public int Out { get; set; }
        public int OnHand { get;  set; }
        public int Worth { get; set; }

        [ForeignKey("Item_ID")]
        [ValidateNever]
        public required Item item { get; set; }
    }
}