using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.User
{
    public class CreateAdminRequestDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]{10}$")]
        public string MobileNumber { get; set; } = string.Empty;
    }
}