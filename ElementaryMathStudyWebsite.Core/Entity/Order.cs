using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Order : BaseEntity
    {
        public required string CustomerId { get; set; }

        public required double TotalPrice { get; set; }

        public required virtual User User { get; set; } // Navigation property, one order has one user

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual User? LastUpdatedByUser { get; set; }
        public virtual User? DeletedByUser { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } // Navigation property, one order has many order detail

        public Order() { }

        public Order(string customerId, double totalPrice)
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
        }
    }
}
