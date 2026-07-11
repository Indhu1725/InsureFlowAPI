using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.Claim
{
    public class ClaimStatusHistoryResponseDto
    {
        public int HistoryId { get; set; }

        public ClaimStatus OldStatus { get; set; }

        public ClaimStatus NewStatus { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public DateTime ChangedDate { get; set; }

        public string ChangedBy { get; set; } = string.Empty;
    }
}