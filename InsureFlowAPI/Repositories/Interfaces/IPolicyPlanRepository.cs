using InsureFlowAPI.Models;
using InsureFlowAPI.DTOs.Common;

namespace InsureFlowAPI.Repositories.Interfaces
{
    public interface IPolicyPlanRepository
    {
        Task<PagedResponse<PolicyPlan>> GetAllAsync(PolicyPlanQueryDto query);

        Task<IEnumerable<PolicyPlan>> GetActivePlansAsync();

        Task<IEnumerable<PolicyPlan>> GetByProductIdAsync(int productId);

        Task<IEnumerable<PolicyPlan>> GetActivePlansByProductIdAsync(int productId);

        Task<PolicyPlan?> GetByIdAsync(int id);

        Task<PolicyPlan?> GetByNameAsync(string planName);

        Task<bool> PlanNameExistsAsync(string planName); 

        Task AddAsync(PolicyPlan plan);

        Task UpdateAsync(PolicyPlan plan);

        Task SoftDeleteAsync(PolicyPlan plan);

        Task SaveChangesAsync();
    }
}