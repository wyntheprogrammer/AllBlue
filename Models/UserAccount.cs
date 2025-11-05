using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("UserAccount")]
    public class UserAccount
    {
        [Key]
        public int User_Account_ID { get; set; }
        public string? Image { get; set; }
        public required string? Firstname { get; set; }
        public required string? Middlename { get; set; }
        public required string? Lastname { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string? Password { get; set; }
        public required string Contact  { get; set; }
        public required string Gender { get; set; }
        public required string Address { get; set; }
        public int Account_Type_ID { get; set; }
        public required string? Status { get; set; }

        [ForeignKey("Account_Type_ID")]
        [ValidateNever]
        public required AccountType accountType { get; set; }
       
    }
}