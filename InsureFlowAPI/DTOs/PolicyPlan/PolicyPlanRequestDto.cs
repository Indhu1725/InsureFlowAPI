using InsureFlowAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.PolicyPlan
{
    public class PolicyPlanRequestDto
    {
        [Required(ErrorMessage = "Product Id is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Plan Name is required.")]
        public string PlanName { get; set; }

        [Required(ErrorMessage = "Coverage Amount is required.")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
            ErrorMessage = "Coverage Amount must be greater than 0.")]
        public decimal CoverageAmount { get; set; }

        [Required(ErrorMessage = "Premium Amount is required.")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
            ErrorMessage = "Premium Amount must be greater than 0.")]
        public decimal PremiumAmount { get; set; }

        [Required(ErrorMessage = "Premium Type is required.")]
        public PremiumType PremiumType { get; set; }

        [Range(1, 100, ErrorMessage = "Duration must be between 1 and 100 years.")]
        public int DurationYears { get; set; }

        [Required(ErrorMessage = "Terms and Conditions are required.")]
        public string TermsAndConditions { get; set; }

        public bool IsActive { get; set; }
    }
}