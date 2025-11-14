using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllBlue.Models
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public required int Payment_ID { get; set; }
        public required int Order_ID { get; set; }
        public required int Total { get; set; }
        public required string Status { get; set; }
        public required DateOnly Date { get; set; }
    }
}
