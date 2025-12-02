namespace AllBlue.Models
{
    public class ConfirmPaymentItem
    {
        public int ItemID { get; set; }
        public int ClientGal { get; set; }
        public int WRSGal { get; set; }
        public int FreeGal { get; set; }
        public int Qty { get; set; }
        public int Total { get; set; }
    }

    public class ConfirmPaymentViewModel
    {
        public int CustomerID { get; set; }
        public int TotalQty { get; set; }
        public int? Free { get; set; }
        public string? Discount { get; set; }
        public int? Cash { get; set; }
        public int? Changed { get; set; }
        public int? Balanced { get; set;}
        public int TotalPrice { get; set; }
        public string SelectedService { get; set; }
        public string Status { get; set; }
        public DateOnly Date { get; set; }

        public int SelectedUserID { get; set; } 
        public List<ConfirmPaymentItem> Items { get; set; }

    }
}
