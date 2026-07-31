using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InsureFlowAPI.DTOs.Customer
{
    public class CustomerRequestDto
    {
        [Required(ErrorMessage = "Date of birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Pin Code is required.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Pin Code must be exactly 6 numeric digits.")]
        public string PinCode { get; set; }

        [Required(ErrorMessage = "Nominee name is required.")]
        public string NomineeName { get; set; }

        [Required(ErrorMessage = "Nominee relation is required.")]
        public string NomineeRelation { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
