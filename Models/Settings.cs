using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("Settings")]
    public class Settings
    {
        [Key]
        public int SettingsID { get; set; }
        public required string Title { get; set; }
        public required string Footer { get; set; }
        public required string System { get; set; }
        public required string Contact1 { get; set; }
        public required string Contact2 { get; set; }
        public required string Contact3 { get; set; }
        public int Barangay_ID { get; set; }
        public int City_ID { get; set; }
        public string  Province { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; set; }
        public int DeliveryInterval { get; set; }
        public string LocalAddress { get; set; }
        public string Favicon { get; set; }
        public string Logo { get; set; }
        public string Banner { get; set; }


        [ForeignKey("Barangay_ID")]
        [ValidateNever]
        public required Barangay barangay { get; set; }

        [ForeignKey("City_ID")]
        [ValidateNever]
        public required City city { get; set; }


    }
}

