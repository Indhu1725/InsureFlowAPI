using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.PolicyPlan
{
    public class PolicyPlanResponseDto
    {
        public int PlanId { get; set; }

        public string ProductName { get; set; }

        public ProductType ProductType { get; set; }

        public string PlanName { get; set; }

        public decimal CoverageAmount { get; set; }

        public decimal PremiumAmount { get; set; }

        public PremiumType PremiumType { get; set; }

        public int DurationYears { get; set; }

        public string TermsAndConditions { get; set; }

        public bool IsActive { get; set; }
    }
}
