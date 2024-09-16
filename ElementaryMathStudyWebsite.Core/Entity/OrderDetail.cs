using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class OrderDetail
    {
        public required string OrderId { get; set; }

        public required string SubjectId { get; set; }

        public required string StudentId { get; set; }

        public virtual Order? Order { get; set; } // Navigation property, one order detail belong to one order
        public virtual User? User { get; set; } // Navigation property, one order detail assigned to one student
        public virtual Subject? Subject { get; set; } // Navigation property, one order detail contain one subject

        public OrderDetail() { }


        public OrderDetail(string orderId, string subjectId, string studentId, Order order, User user)
        {
            OrderId = orderId;
            SubjectId = subjectId;
            StudentId = studentId;
            Order = order;
            User = user;
        }
    }
}
