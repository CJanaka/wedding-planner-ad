namespace wedding_planer_ad.Models
{
    public class WeddingChecklist
    {
        public int Id { get; set; }
        public int CoupleId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStatus { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
