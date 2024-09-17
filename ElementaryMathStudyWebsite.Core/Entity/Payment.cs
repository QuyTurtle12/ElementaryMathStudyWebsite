using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class Payment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string CustomerId { get; set; }

        public required string OrderId { get; set; }

        public DateTimeOffset PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public double Amount { get; set; }

        public string Status { get; set; } = string.Empty; // Pending, Success, Failed

        // Navigation Properties
        public virtual Order? Order { get; set; } // Navigation property, one payment associated with one order
        public virtual User? User { get; set; } // Navigation property, one payment associated with one order

    }
}
