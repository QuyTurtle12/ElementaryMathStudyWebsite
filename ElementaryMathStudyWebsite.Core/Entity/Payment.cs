using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class Payment
    {
        [Key]
        public string Id { get; set; }

        public required string CustomerId { get; set; }

        public required string OrderId { get; set; }

        public DateTimeOffset PaymentDate { get; set; }

        public string PaymentMethod { get; set; }

        public double Amount { get; set; }

        public string Status { get; set; } // Pending, Success, Failed

        // Navigation Properties
        public virtual Order? Order { get; set; } // Navigation property, one payment associated with one order
        public virtual User? User { get; set; } // Navigation property, one payment associated with one order

        public Payment() { }

        public Payment(string customerId, string orderId, DateTimeOffset paymentDate, string paymentMethod, double amount, string status)
        {
            Id = Guid.NewGuid().ToString();
            CustomerId = customerId;
            OrderId = orderId;
            PaymentDate = paymentDate;
            PaymentMethod = paymentMethod;
            Amount = amount;
            Status = status;
        }
    }
}
