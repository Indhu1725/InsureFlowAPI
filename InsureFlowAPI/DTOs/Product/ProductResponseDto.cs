using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.Product
{
    public class ProductResponseDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public ProductType ProductType { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
