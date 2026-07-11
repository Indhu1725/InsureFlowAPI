using InsureFlowAPI.Models;

namespace InsureFlowAPI.Repositories.Interfaces
{
    public interface IClaimDocumentRepository
    {
        Task<IEnumerable<ClaimDocument>> GetAllAsync();

        Task<IEnumerable<ClaimDocument>> GetByClaimIdAsync(int claimId);

        Task<ClaimDocument?> GetByIdAsync(int id);

        Task AddAsync(ClaimDocument claimDocument);

        Task SaveChangesAsync();
    }
}