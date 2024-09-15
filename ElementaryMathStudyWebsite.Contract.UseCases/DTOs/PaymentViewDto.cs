using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class PaymentViewDto
    {
        public required string PaymentId { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }

    }
}
