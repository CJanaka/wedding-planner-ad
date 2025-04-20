namespace wedding_planer_ad.Models
{
    public class WeddingPlanner
    {
        public int Id { get; set; }
        public string PlannerUserId { get; set; }
        public int CoupleId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsDeleted { get; set; }
        public Couple? Couple { get; set; }
    }
}
