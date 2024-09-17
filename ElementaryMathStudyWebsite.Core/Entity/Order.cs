using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Order : BaseEntity
    {
        public string CustomerId { get; set; } = string.Empty;

        public double TotalPrice { get; set; } = 0;

        // Navigation properties
        [JsonIgnore]
        public virtual User? CreatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? LastUpdatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? DeletedByUser { get; set; }
        [JsonIgnore]
        public virtual User? User { get; set; } // Navigation property, one order has one user
        [JsonIgnore]
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } // Navigation property, one order has many order detail
        [JsonIgnore]
        public virtual Payment? Payment { get; set; } // Navigation property, one order associated with one payment

    }
}
