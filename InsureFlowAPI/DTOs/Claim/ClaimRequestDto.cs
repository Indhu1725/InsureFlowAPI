using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Claim
{
    public class ClaimRequestDto
    {
        [Required(ErrorMessage = "Policy is required.")]
        public int PolicyId { get; set; }

        [Range(1, double.MaxValue,
            ErrorMessage = "Claim amount must be greater than zero.")]
        public decimal ClaimAmount { get; set; }

        [Required(ErrorMessage = "Claim Reason is Required")]
        [StringLength(500)]
        public string ClaimReason { get; set; } = string.Empty;

        [Required(ErrorMessage = "IncidentDate is Required")]
        public DateOnly IncidentDate { get; set; }
    }
}