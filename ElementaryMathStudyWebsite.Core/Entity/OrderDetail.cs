using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class OrderDetail
    {
        public string OrderId { get; set; } = string.Empty;

        public string SubjectId { get; set; } = string.Empty;

        public string StudentId { get; set; } = string.Empty;

        public virtual Order Order { get; set; } = new(); // Navigation property, one order detail belong to one order
        public virtual User User { get; set; } = new(); // Navigation property, one order detail assigned to one student
        public virtual Subject Subject { get; set; } = new(); // Navigation property, one order detail contain one subject

    }
}
