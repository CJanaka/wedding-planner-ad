namespace wedding_planer_ad.Models.ViewModels
{
    public class VendorBookingViewModel
    {
        public List<Vendor> Vendors { get; set; }
        public List<VendorCat> Categories { get; set; }
        public string SearchString { get; set; }
        public int? CategoryId { get; set; }
        public string Location { get; set; }
        public string SortOrder { get; set; }
    }
}
