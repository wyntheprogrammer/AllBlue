using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    public class ExpensesHistoryItemViewModel
    {
        public int ExpenseID { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public string Transaction { get; set; }
        public string Date { get; set; }
    }


}

