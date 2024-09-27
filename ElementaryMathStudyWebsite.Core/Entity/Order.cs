using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Order : BaseEntity
    {
        public string CustomerId { get; set; } = string.Empty;

        public double TotalPrice { get; set; } = 0;

        public string PaymentMethod { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending"; // Pending/Success/Failed

        // Navigation properties
        [JsonIgnore]
        public virtual User User { get; set; } = new(); // Navigation property, one order has one user
        [JsonIgnore]
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } // Navigation property, one order has many order detail

    }
}
