using InsureFlowAPI.DTOs.Common;

namespace InsureFlowAPI.DTOs.Customer
{
    public class CustomerQueryDto : PaginationRequestDto
    {
        public bool? IsActive { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }
    }
}