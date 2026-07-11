using InsureFlowAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsureFlowAPI.DTOs.Payment
{
    public class PremiumPaymentRequestDto
    {
        [Required]
        public int PolicyId { get; set; }

        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMode PaymentMode { get; set; }

        [Required]
        public string TransactionReference { get; set; }

    }
}
