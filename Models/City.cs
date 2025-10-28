using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("City")]
    public class City
    {
        [Key]
        public int City_ID { get; set; }
        public required string Name { get; set; }

    }
}

