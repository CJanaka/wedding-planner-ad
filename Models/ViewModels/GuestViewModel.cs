namespace wedding_planer_ad.Models.ViewModels
{
    public class GuestViewModel
    {
        public List<Guest>? Guests { get; set; }
        public Guest? Guest { get; set; }
        public int CoupleId { get; set; }
    }
}
