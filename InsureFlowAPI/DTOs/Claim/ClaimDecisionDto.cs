using InsureFlowAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Claim
{
    public class ClaimDecisionDto
    {
        [Required]
        public ClaimStatus FinalDecisionStatus { get; set; }

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; }
    }
}
