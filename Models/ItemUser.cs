namespace AllBlue.Models
{
    public class ItemUser
    {
        public List<Item> Item { get; set; }
        public List<UserAccount> UserAccount { get; set; }

        public int? UserAccountID { get; set; }
    }
}
