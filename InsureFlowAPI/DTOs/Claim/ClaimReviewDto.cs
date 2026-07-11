using InsureFlowAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Claim
{ 
    public class ClaimReviewDto
    {
        [Required]
        public ClaimStatus RecommendedStatus { get; set; }

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; }
    }
}
