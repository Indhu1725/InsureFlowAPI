using InsureFlowAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Product
{
    public class ProductRequestDto
    {
        [Required(ErrorMessage ="Product Name is Required")]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required(ErrorMessage ="Prodyct Type is Required")]
        public ProductType ProductType { get; set; }

        [Required(ErrorMessage ="Description is Required")]
        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}