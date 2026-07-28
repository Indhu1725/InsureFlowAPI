namespace InsureFlowAPI.DTOs.Payment
{
    public class PremiumDueResponseDto
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateOnly? NextPremiumDueDate { get; set; }
        public DateTime? LastPremiumPaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}