using InsureFlowAPI.DTOs.Common;
using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.User
{
    public class UserQueryDto : PaginationRequestDto
    {
        public Role? Role { get; set; }

        public bool? IsActive { get; set; }
    }
}