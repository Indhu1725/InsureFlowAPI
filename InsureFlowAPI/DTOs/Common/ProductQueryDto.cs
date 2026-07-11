using InsureFlowAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Common
{
    public class ProductQueryDto
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public ProductType? ProductType { get; set; }

        public bool? IsActive { get; set; }

        public string SortBy { get; set; } = "ProductName";

        public bool Descending { get; set; } = false;
    }
}