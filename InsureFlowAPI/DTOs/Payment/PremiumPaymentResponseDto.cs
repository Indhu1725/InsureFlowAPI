using InsureFlowAPI.Models.Enums;

namespace InsureFlowAPI.DTOs.Payment
{
    public class PremiumPaymentResponseDto
    {
        public int PaymentId { get; set; }

        public string PolicyNumber { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentMode PaymentMode { get; set; }

        public string TransactionReference { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
    }
}
