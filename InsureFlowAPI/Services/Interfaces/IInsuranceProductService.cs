using InsureFlowAPI.DTOs.Common;
using InsureFlowAPI.DTOs.Product;

namespace InsureFlowAPI.Services.Interfaces
{
    public interface IInsuranceProductService
    {
        Task<PagedResponse<ProductResponseDto>> GetAllProductsAsync(ProductQueryDto query);

        Task<IEnumerable<ProductResponseDto>> GetActiveProductsAsync();

        Task<ProductResponseDto?> GetProductByIdAsync(int id);

        Task<ProductResponseDto?> GetProductByNameAsync(string productName);  

        Task<ProductResponseDto> CreateProductAsync(ProductRequestDto requestDto);

        Task<ProductResponseDto> UpdateProductAsync(int id, ProductRequestDto requestDto);

        Task SoftDeleteProductAsync(int id);
    }
}
