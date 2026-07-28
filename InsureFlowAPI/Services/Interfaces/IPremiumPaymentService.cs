using InsureFlowAPI.DTOs.Payment;
using InsureFlowAPI.DTOs.Common;

namespace InsureFlowAPI.Services.Interfaces
{
    public interface IPremiumPaymentService
    {
        Task<PagedResponse<PremiumPaymentResponseDto>> GetAllPaymentsAsync(PaginationRequestDto paginationDto);
        Task<PagedResponse<PremiumPaymentResponseDto>> GetPaymentsByPolicyIdAsync(int policyId,PaginationRequestDto paginationDto);

        Task<PagedResponse<PremiumPaymentResponseDto>> GetPaymentsByCustomerIdAsync(int customerId,int userId,string role,PaginationRequestDto paginationDto);

        Task<PremiumPaymentResponseDto?> GetPaymentByIdAsync(int id);

        Task<PremiumPaymentResponseDto> MakePaymentAsync(PremiumPaymentRequestDto requestDto,int userId,string role);
        Task<PremiumDueResponseDto> GetPremiumDueAsync(int userId);
        Task<PagedResponse<PremiumPaymentResponseDto>> GetMyPaymentsAsync(
    int userId,
    PaginationRequestDto paginationDto);
    }
}