namespace AllBlue.Models
{
    public class CustomerBarangayCityViewModel
    {
        public List<Customer> Customers { get; set; }
        public List<Barangay> Barangay { get; set; }
        public List<City> City { get; set; }
        public int? SelectedBarangayID { get; set; }
        public int? SelectedCityID { get; set; }
    }

}

