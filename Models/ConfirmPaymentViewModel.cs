namespace AllBlue.Models
{
    public class ConfirmPaymentItem
    {
        public int ItemID { get; set; }
        public int ClientGal { get; set; }
        public int WRSGal { get; set; }
        public int FreeGal { get; set; }
        public int Qty { get; set; }
        public string Total { get; set; }
    }

    public class ConfirmPaymentViewModel
    {
        public int CustomerID { get; set; }
        public int SelectedUserID { get; set; } 
        public string SelectedService { get; set; }
        public string? Status { get; set; }
        public List<ConfirmPaymentItem> Items { get; set; }
        public DateOnly Date { get; set; }
        public string TotalQty { get; set; }
        public string TotalPrice { get; set; }
        public string? Changed { get; set; }

    }
}
