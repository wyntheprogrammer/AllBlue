using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    public class ExpensesHistoryViewModel
    {
        public string CategoryName { get; set; }              // category title
        public List<ExpensesHistoryItemViewModel> Items { get; set; }  // all expense rows
    }


}

