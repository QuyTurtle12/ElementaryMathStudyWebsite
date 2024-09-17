namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Progress
    {
        public required string StudentId { get; set; }
        public string? QuizId { get; set; }
        public string? SubjectId { get; set; }

        public virtual User? User { get; set; } // Navigation property, one progress belong to one subjects
        public virtual Quiz? Quiz { get; set; } // Navigation property, one progress has one completed quiz
                                                // This attribute use for checking progress in topic
        public virtual Subject? Subject { get; set; } // Navigation property, one progress has one subject

    }
}
