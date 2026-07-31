using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Authentication
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage ="Enter valid Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [Phone(ErrorMessage = "Invalid mobile number format.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string MobileNumber { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}
