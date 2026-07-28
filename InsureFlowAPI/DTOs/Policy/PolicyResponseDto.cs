using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.Policy
{
    public class PolicyResponseDto
    {
        public int PolicyId { get; set; }

        public string PolicyNumber { get; set; }

        public string CustomerName { get; set; }

        public string PlanName { get; set; }

        public string ProductType { get; set; } = string.Empty;

        public decimal CoverageAmount { get; set; }

        public decimal PremiumAmount { get; set; }

        public PremiumType PremiumType { get; set; } 

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string PolicyStatus { get; set; } = string.Empty;

        public decimal TotalPremiumPaid { get; set; }
    }
}
