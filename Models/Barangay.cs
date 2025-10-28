using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("Barangay")]
    public class Barangay
    {
        [Key]
        public int Barangay_ID { get; set; }
        public required string Name { get; set; }
        public int City_ID { get; set; }
        public required string Color { get; set; }


        [ForeignKey("City_ID")]
        [ValidateNever]
        public required City city { get; set; }


    }
}

