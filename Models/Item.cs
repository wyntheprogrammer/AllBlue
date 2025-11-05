using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllBlue.Models
{
    [Table("Item")]
    public class Item
    {
        [Key]
        public int Item_ID { get; set; }
        public string? Image { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Item_Type { get; set; }
        public int POS_Item { get; set; }
        public int Reorder { get; set; }
        public int Deliver { get; set; }
        public int Pickup { get; set; }
        public int Buy { get; set; }
        public DateTime Date { get; set; }

    }
}

