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
        public required string Image { get; set; }
        public required string Title { get; set; }
        public required string Item_Type { get; set; }
        public required int POS_Item { get; set; }
        public required int Reorder { get; set; }
        public required int Deliver { get; set; }
        public required int Pickup { get; set; }
        public required int Buy { get; set; }
        public required DateTime Date { get; set; }

    }
}

