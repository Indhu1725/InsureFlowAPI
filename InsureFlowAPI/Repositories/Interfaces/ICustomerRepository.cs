using InsureFlowAPI.DTOs.Customer;
using InsureFlowAPI.Models;
using InsureFlowAPI.Repositories.Common;

namespace InsureFlowAPI.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<PagedResult<Customer>> GetAllAsync(CustomerQueryDto query);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task<Customer?> GetByUserIdAsync(int userId);

        Task AddAsync(Customer customer);

        Task UpdateAsync(Customer customer);

        Task SaveChangesAsync();
    }
}