using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.Payment
{
    public class PremiumPaymentResponseDto
    {
        public int PaymentId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PolicyNumber { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMode { get; set; } = string.Empty;

        public string TransactionReference { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;
    }
}
