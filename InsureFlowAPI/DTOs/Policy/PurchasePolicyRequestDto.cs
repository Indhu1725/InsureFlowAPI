using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Policy
{
    public class PurchasePolicyRequestDto
    {
        [Required]
        public int PlanId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }
    }
}
