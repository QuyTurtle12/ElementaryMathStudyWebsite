using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Quiz : BaseEntity
    {
        public string QuizName { get; set; } = string.Empty;

        public double? Criteria { get; set; } // Mark that need to be achieved to passed

        public bool Status { get; set; }

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual User? LastUpdatedByUser { get; set; }
        public virtual User? DeletedByUser { get; set; }
        public virtual Chapter? Chapter { get; set; } // Navigation property, one quiz belong to chapter
        public virtual Topic? Topic { get; set; } // Navigation property, one quiz belong to one topic
        public virtual ICollection<Question>? Questions { get; set; } // Navigation property, one quiz has many question
        public virtual ICollection<Progress>? Progresses { get; set; } // Navigation property, one quiz can be in many progresses

        public Quiz() { }

        public Quiz(string quizName, double? criteria)
        {
            QuizName = quizName;
            Status = true; // Always true when initialized
            Criteria = criteria;
        }
    }
}
