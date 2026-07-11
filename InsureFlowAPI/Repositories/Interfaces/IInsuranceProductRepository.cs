using InsureFlowAPI.DTOs.Common;
using InsureFlowAPI.Models;

namespace InsureFlowAPI.Repositories.Interfaces
{
    public interface IInsuranceProductRepository
    {
        Task<PagedResponse<InsuranceProduct>> GetAllAsync(ProductQueryDto query);

        Task<IEnumerable<InsuranceProduct>> GetActiveProductsAsync();

        Task<InsuranceProduct?> GetByIdAsync(int id);

        Task<InsuranceProduct?> GetByNameAsync(string productName);

        Task<bool> ProductNameExistsAsync(string productName);

        Task AddAsync(InsuranceProduct product);

        Task UpdateAsync(InsuranceProduct product);

        Task SoftDeleteAsync(InsuranceProduct product);

        Task SaveChangesAsync();
    }
}