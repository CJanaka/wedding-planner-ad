using System.ComponentModel.DataAnnotations;

namespace wedding_planer_ad.Models.DTO
{
    public class NewVendorDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\W).+$", ErrorMessage = "Password must contain a lowercase letter and a special character.")]
        public string? Password { get; set; }
        public string? NewPassword { get; set; }

    }
}
