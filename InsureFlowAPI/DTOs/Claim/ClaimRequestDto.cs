using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Claim
{ 
    public class ClaimRequestDto
    {
        [Required]
        public int PolicyId { get; set; }

        [Range(1, double.MaxValue)]
        public decimal ClaimAmount { get; set; }

        [Required(ErrorMessage ="Claim Reason is Required")]
        [StringLength(500)]
        public string ClaimReason { get; set; }

        [Required(ErrorMessage ="IncidentDate is Required")]
        public DateOnly IncidentDate { get; set; }

        [Required(ErrorMessage = "Supporting documents are required.")]
        [MinLength(1, ErrorMessage = "At least one supporting document is required.")]
        public List<SupportingDocumentDto> SupportingDocuments { get; set; }
    = new();
    }
}
