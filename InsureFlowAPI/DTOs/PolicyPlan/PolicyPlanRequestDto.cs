using InsureFlowAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.PolicyPlan
{
    public class PolicyPlanRequestDto
    {
        [Required(ErrorMessage ="ProductId is Required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage ="PlanName is Required")]
        public string PlanName { get; set; }

        [Range(1, double.MaxValue)]
        public decimal CoverageAmount { get; set; }

        [Range(1, double.MaxValue)]
        public decimal PremiumAmount { get; set; }

        [Required]
        public PremiumType PremiumType { get; set; }

        [Range(1, 100)]
        public int DurationYears { get; set; }

        [Required(ErrorMessage ="Terms and Conditions required")]
        public string TermsAndConditions { get; set; }

        public bool IsActive { get; set; }
    }
}
