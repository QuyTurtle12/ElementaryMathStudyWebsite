using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Repositories.Entity
{
    public class Order : BaseEntity
    {
        public required string CustomerId { get; set; }

        public required double TotalPrice { get; set; }

        public required virtual User User { get; set; } // Navigation property, one order has one user
        public required virtual ICollection<OrderDetail> OrderDetails { get; set; } // Navigation property, one order has many order detail
                                                                            
        public Order() { }

        public Order(string customerId, double totalPrice)
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
        }
    }
}
