using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("AccountType")]
    public class AccountType
    {
        [Key]
        public int Account_Type_ID {get; set;}
        public required string Title {get; set;}
        public required string Description {get; set;}
        public required string Menu {get; set;}
    }
}