using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("Expense")]
    public class Expense
    {
        [Key]
        public int ExpenseID { get; set; }
        public int  ExpenseCategoryID { get; set; }
        public int TotalValue { get; set; }
        public DateOnly Date { get; set; }
        public string Comment { get;  set; }

        [ForeignKey("ExpenseCategoryID")]
        [ValidateNever]
        public required ExpenseCategory expenseCategory { get; set; }
    }
}