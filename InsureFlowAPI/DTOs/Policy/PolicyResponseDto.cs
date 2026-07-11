using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.Policy
{
    public class PolicyResponseDto
    {
        public int PolicyId { get; set; }

        public string PolicyNumber { get; set; }

        public string CustomerName { get; set; }

        public string PlanName { get; set; }

        public ProductType ProductType { get; set; }

        public decimal CoverageAmount { get; set; }

        public decimal PremiumAmount { get; set; }

        public PremiumType PremiumType { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public PolicyStatus PolicyStatus { get; set; }

        public decimal TotalPremiumPaid { get; set; }
    }
}
