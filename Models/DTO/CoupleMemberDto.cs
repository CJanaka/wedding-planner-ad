namespace wedding_planer_ad.Models.DTO
{
    public class CoupleMemberDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
