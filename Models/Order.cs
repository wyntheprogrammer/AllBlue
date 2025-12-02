using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Numerics;

namespace AllBlue.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int Order_ID { get; set; }
        public int Payment_ID { get; set; }
        public int Customer_ID { get; set; }
        public int User_Account_ID { get; set; }
        public int Item_ID { get; set; }
        public int? Client_Gal { get; set; }
        public int? WRS_Gal { get; set; }
        public int? Free_Gal { get; set; }
        public int? Quantity { get; set; }
        public int? Total { get; set; }

        [ForeignKey("Payment_ID")]
        [ValidateNever]
        public Payment payment { get; set; }

        [ForeignKey("Customer_ID")]
        [ValidateNever]
        public Customer customer { get; set; }

        [ForeignKey("User_Account_ID")]
        [ValidateNever]
        public UserAccount userAccount { get; set; }

        [ForeignKey("Item_ID")]
        [ValidateNever]
        public Item item { get; set; }

        internal object First()
        {
            throw new NotImplementedException();
        }
    }
}
