using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Repositories.Entity
{
    public class Subject : BaseEntity
    {
        public string SubjectName { get; set; } = string.Empty; // avoid null reference issues

        public double? Price { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<OrderDetail>? Detail { get; set; } // Navigation property, one subject may belong to many order detail
        public virtual ICollection<Chapter>? Chapters { get; set; } // Navigation property, one subject has many chapters
        public virtual ICollection<Progress>? Progresses { get; set; } // Navigation property, one subject has many progresses

        public Subject() { }

        public Subject(string subjectName, double? price)
        {
            SubjectName = subjectName;
            Price = price;
            Status = true; // Always true when initialized
        }
    }
}
