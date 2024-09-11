using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Repositories.Entity
{
    public class Chapter : BaseEntity
    {
        public int? Number { get; set; } // Number use for arranging the chapter orderly

        public string ChapterName { get; set; } = string.Empty; // avoid null reference issues

        public bool Status { get; set; }

        public required string SubjectId { get; set; }

        public string? QuizId { get; set; } // Quiz may not exist yet and can be created later

        public double? Criteria { get; set; } // Mark that need to be achieved to passed

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual User? LastUpdatedByUser { get; set; }
        public virtual User? DeletedByUser { get; set; }
        public virtual Subject? Subject { get; set; } // Navigation property, one chapter belong to one subject
        public virtual ICollection<Topic>? Topics { get; set; } // Navigation property, one chapter has many topics
        public virtual Quiz? Quiz { get; set; } // Navigation property, one chapter can only has one quiz

        public Chapter() { }

        public Chapter(int? number, string chapterName, string? quizId, double? criteria, string subjectId)
        {
            Number = number;
            ChapterName = chapterName;
            Status = true; // Always true when initialized
            SubjectId = subjectId;
            QuizId = quizId;
            Criteria = criteria;
        }
    }
}
