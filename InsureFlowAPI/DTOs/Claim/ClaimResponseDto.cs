using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.Claim
{
    public class ClaimResponseDto
    {
        public int ClaimId { get; set; }

        public string ClaimNumber { get; set; }

        public string PolicyNumber { get; set; }

        public string CustomerName { get; set; }

        public decimal ClaimAmount { get; set; }

        public string ClaimReason { get; set; }

        public DateOnly IncidentDate { get; set; }

        public ClaimStatus ClaimStatus { get; set; }

        public string InternalStaffRemarks { get; set; }

        public string AdminRemarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
