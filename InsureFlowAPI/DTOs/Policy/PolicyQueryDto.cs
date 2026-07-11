using InsureFlowAPI.DTOs.Common;
using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.Policy
{
    public class PolicyQueryDto : PaginationRequestDto
    {
        public PolicyStatus? Status { get; set; }

        public int? CustomerId { get; set; }

        public int? PlanId { get; set; }
    }
}