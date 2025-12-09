using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("ExpenseCategory")]
    public class ExpenseCategory
    {
        [Key]
        public int ExpenseCategoryID { get; set; }
        public string Name { get;  set; }
        public string Image { get; set; }

        [ValidateNever]
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}