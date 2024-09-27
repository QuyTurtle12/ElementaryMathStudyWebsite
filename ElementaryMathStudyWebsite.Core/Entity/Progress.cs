using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Progress
    {
        public string StudentId { get; set; } = string.Empty;
        public string QuizId { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;

        public virtual User? User { get; set; } // Navigation property, one progress belong to one subjects
        public virtual Quiz? Quiz { get; set; } // Navigation property, one progress has one completed quiz
                                                // This attribute use for checking progress in topic
        public virtual Subject? Subject { get; set; } // Navigation property, one progress has one subject

    }
}
