using InsureFlowAPI.DTOs.PolicyPlan;
using InsureFlowAPI.DTOs.Common;

namespace InsureFlowAPI.Services.Interfaces
{
    public interface IPolicyPlanService
    {
        Task<PagedResponse<PolicyPlanResponseDto>> GetAllPlansAsync(PolicyPlanQueryDto query);

        Task<IEnumerable<PolicyPlanResponseDto>> GetActivePlansAsync();

        Task<IEnumerable<PolicyPlanResponseDto>> GetPlansByProductIdAsync(int productId);

        Task<IEnumerable<PolicyPlanResponseDto>> GetActivePlansByProductIdAsync(int productId);

        Task<PolicyPlanResponseDto?> GetPlanByIdAsync(int id);

        Task<PolicyPlanResponseDto> CreatePlanAsync(PolicyPlanRequestDto requestDto);

        Task UpdatePlanAsync(int id, PolicyPlanRequestDto requestDto);

        Task SoftDeletePlanAsync(int id);
    }
}


