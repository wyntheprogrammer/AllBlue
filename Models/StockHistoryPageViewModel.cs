using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
   public class StockHistoryPageViewModel
    {
        public Stock Stock { get; set; }               
        public List<StockHistoryViewModel> History { get; set; } = new(); 
    }

}