using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace AllBlue.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int Order_ID { get; set; }
        public required int Customer_ID { get; set; }
        public required int User_Account_ID { get; set; }
        public required int Item_ID { get; set; }
        public required int Price { get; set; }
        public int? Client_Gal { get; set; }
        public int? WRS_Gal { get; set; }
        public int? Free_Gal { get; set; }
        public required int Total { get; set; }
    }
}
