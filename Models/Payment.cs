using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllBlue.Models
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public int Payment_ID { get; set; }
        public string Total { get; set; }
        public string Status { get; set; }
        
        public int? Quantity { get; set; }
        public string? Service { get; set; }
        public DateOnly Date { get; set; }

        public ICollection<Order> Orders { get; set; }

    }
}
