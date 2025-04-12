using System.ComponentModel.DataAnnotations;

namespace wedding_planer_ad.Models.DTO
{
    public class AdminUserDto
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d)(?=.*[\W_]).*$", ErrorMessage = "Password must contain at least one lowercase letter, one number, and one non-alphanumeric character.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password (optional)")]
        public string? NewPassword { get; set; }
    }
}
