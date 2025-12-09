using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    public class NeedDeliver 
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Service { get; set; }
        public decimal LastSales { get; set; }
        public decimal LastUnpaid { get; set; }
        public int ClientGallon { get; set; }
        public int WRSGallon { get; set; }
        public DateOnly? LastPurchased { get; set; }
        public int BarangayID { get; set; }
    }
}
