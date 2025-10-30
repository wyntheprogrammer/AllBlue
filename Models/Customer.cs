using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public int Customer_ID { get; set; }
        public required string First_Name { get; set; }
        public required string Last_Name { get; set; }
        public string Gender { get; set; }
        public string Alias { get; set; }
        public DateOnly Birth { get; set; }
        public string House_No { get; set; }
        public string Street { get; set; }
        public int Barangay_ID { get; set; }
        public int City_ID { get; set; }
        public string Province { get; set; }
        public int Item_ID { get; set; }
        public int Client_Gallon { get; set; }
        public int WRS_Gallon { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("Barangay_ID")]
        [ValidateNever]
        public required Barangay barangay { get; set; }

        [ForeignKey("City_ID")]
        [ValidateNever]
        public required City city { get; set; }

        [ForeignKey("Item_ID")]
        [ValidateNever]
        public required Item item { get; set; }


    }
}

